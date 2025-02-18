﻿using Lacos.GestioneCommesse.Dal;
using AutoMapper;
using Lacos.GestioneCommesse.Application.CheckLists.DTOs;
using Lacos.GestioneCommesse.Domain.Registry;
using Lacos.GestioneCommesse.Framework.Exceptions;
using Lacos.GestioneCommesse.Framework.Extensions;
using Microsoft.EntityFrameworkCore;
using Lacos.GestioneCommesse.Application.Docs.DTOs;
using Lacos.GestioneCommesse.Domain.Docs;
using System.Diagnostics;

namespace Lacos.GestioneCommesse.Application.CheckLists.Services
{

    public interface ICheckListService
    {
        IQueryable<CheckListDto> GetCheckList();
        Task<CheckListDto> GetCheckListDetail(long id);
        Task UpdateCheckList(long id, CheckListDto checkListDto);
        Task<CheckListDto> CreateCheckList(CheckListDto checkListDto);
        Task DeleteCheckList(long id);
        Task<IEnumerable<CheckListItemDto>> GetCheckListItems(long checklistId);
        Task<CheckListItemDto> GetCheckListItemDetail(long checkListItemId);
        Task UpdateCheckListItem(long id, CheckListItemDto checkListItemDto);
        Task DeleteCheckListItem(long id);
        Task<CheckListItemDto> CreateCheckListItem(CheckListItemDto checkListItemDto);
        Task<CheckListDto> CopyChecklist(ChecklistCopyDto checklistCopyDto);
    }

    public class CheckListService : ICheckListService
    {
        private readonly IMapper mapper;
        private readonly IRepository<Domain.Registry.CheckList> checklistRepository;
        private readonly IRepository<CheckListItem> checklistItemRepository;
        private readonly ILacosDbContext dbContext;


        public CheckListService(
            IMapper mapper,
            IRepository<Domain.Registry.CheckList> checklistRepository,
            ILacosDbContext dbContext, IRepository<CheckListItem> checklistItemRepository)
        {
            this.mapper = mapper;
            this.checklistRepository = checklistRepository;
            this.dbContext = dbContext;
            this.checklistItemRepository = checklistItemRepository;
        }

        public IQueryable<CheckListDto> GetCheckList()
        {
            var checklists = checklistRepository
                .Query()
                .AsNoTracking()
                .Project<CheckListDto>(mapper);

            return checklists;
        }

        public async Task<CheckListDto> GetCheckListDetail(long id)
        {
            if (id == 0)
                throw new LacosException("Impossibile recuperare una checklist con id 0");

            var checklist = await checklistRepository
                .Query()
                .AsNoTracking()
                .Include(x => x.ProductType)
                .Include(x => x.ActivityType)
                .Include(x => x.Items)
                .Where(x => x.Id == id)
                .SingleOrDefaultAsync();

            if (checklist == null)
                throw new LacosException($"Impossibile trovare la checklist con id {id}");

            return checklist.MapTo<CheckListDto>(mapper);
        }

        public async Task UpdateCheckList(long id, CheckListDto checkListDto)
        {
            if (id == 0)
                throw new LacosException("Impossibile aggiornare una checklist con id 0");

            var checklist = await checklistRepository
                .Query()
                .Where(x => x.Id == id)
                .Include(x => x.Items)
                .SingleOrDefaultAsync();

            if (checklist == null)
                throw new LacosException($"Impossibile trovare una checklist con id {id}");

            checkListDto.MapTo(checklist, mapper);
            await dbContext.SaveChanges();
        }

        public async Task<CheckListDto> CreateCheckList(CheckListDto checkListDto)
        {
            var checklist = checkListDto.MapTo<Domain.Registry.CheckList>(mapper);
            await checklistRepository.Insert(checklist);
            
            try
            {
                await dbContext.SaveChanges();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            return checklist.MapTo<CheckListDto>(mapper);
        }

        public async Task DeleteCheckList(long id)
        {
            if (id == 0)
                throw new LacosException("Impossible eliminare una checklist con id 0");

            var checklist = await checklistRepository
                .Query()
                .Where(x => x.Id == id)
                .SingleOrDefaultAsync();

            if (checklist == null)
                throw new LacosException($"Impossibile trovare una checklist con id {id}");

            checklistRepository.Delete(checklist);
          
            await dbContext.SaveChanges();
        }

        public async Task<IEnumerable<CheckListItemDto>> GetCheckListItems(long checklistId)
        {
            var checkListItems = await checklistItemRepository
                .Query()
                .AsNoTracking()
                .Where(x => x.CheckListId == checklistId)
                .ToListAsync();

            return checkListItems.MapTo<IEnumerable<CheckListItemDto>>(mapper);
        }

        public async Task<CheckListItemDto> GetCheckListItemDetail(long checkListItemId)
        {
            if (checkListItemId == 0)
                throw new LacosException("Impossibile recuperare un item con id 0");

            var checkListItem = await checklistItemRepository
                .Query()
                .AsNoTracking()
                
                .Where(x => x.Id == checkListItemId)
                .SingleOrDefaultAsync();

            if (checkListItem == null)
                throw new LacosException($"Impossibile trovare un item con id {checkListItemId}");

            return checkListItem.MapTo<CheckListItemDto>(mapper);

        }
        public async Task UpdateCheckListItem(long id, CheckListItemDto checkListItemDto)
        {
            if (id == 0)
                throw new LacosException("Impossibile aggiornare un item con id 0");

            var checkListItem = await checklistItemRepository
                .Query()
                .Where(x => x.Id == id)
                .SingleOrDefaultAsync();

            if (checkListItem == null)
                throw new LacosException($"Impossibile trovare un item  con id {id}");

            checkListItemDto.MapTo(checkListItem, mapper);
            await dbContext.SaveChanges();
        }

        public async Task<CheckListItemDto> CreateCheckListItem(CheckListItemDto checkListItemDto)
        {
            var checkListItem = checkListItemDto.MapTo<CheckListItem>(mapper);
            await checklistItemRepository.Insert(checkListItem);
            
            try
            {
                await dbContext.SaveChanges();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            return checkListItem.MapTo<CheckListItemDto>(mapper);
        }

        public async Task DeleteCheckListItem(long id)
        {
            if (id == 0)
                throw new LacosException("Impossible eliminare un item con id 0");

            var checkListItem = await checklistItemRepository
                .Query()
                .Where(x => x.Id == id)
                .SingleOrDefaultAsync();

            if (checkListItem == null)
                throw new LacosException($"Impossibile trovare un item con id {id}");

            checklistItemRepository.Delete(checkListItem);
          
            await dbContext.SaveChanges();
        }

        public async Task<CheckListDto> CopyChecklist(ChecklistCopyDto checklistCopyDto)
        {
            var sourceChecklist = await checklistRepository.Query()
                .AsNoTracking()
                .Where(x => x.Id == checklistCopyDto.SourceChecklistId)
                .Include(x => x.Items)
                .FirstOrDefaultAsync();

            if (sourceChecklist == null)
                throw new LacosException($"Impossibile trovare checklist con id {checklistCopyDto.SourceChecklistId}");

            CheckList checkList = new CheckList();

            checkList.ActivityTypeId = checklistCopyDto.ActivityTypeId;
            checkList.ProductTypeId = checklistCopyDto.ProductTypeId;
            checkList.Description = sourceChecklist.Description;

            await checklistRepository.Insert(checkList);

            await dbContext.SaveChanges();

            foreach (CheckListItem checkListItem in sourceChecklist.Items)
            {
                CheckListItem newCheckListItem = new CheckListItem();
                newCheckListItem.CheckListId = checkList.Id;
                newCheckListItem.Description = checkListItem.Description;
                await checklistItemRepository.Insert(newCheckListItem);

            }
            await dbContext.SaveChanges();

            return await GetCheckListDetail(checkList.Id);
        }
    }
}
