using Lacos.GestioneCommesse.Dal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Lacos.GestioneCommesse.Application.CheckList.DTOs;
using Lacos.GestioneCommesse.Framework.Session;
using Lacos.GestioneCommesse.Domain.Registry;
using Lacos.GestioneCommesse.Framework.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Lacos.GestioneCommesse.Application.CheckList
{

    public interface ICheckListService
    {
        IQueryable<CheckListDto> GetCheckList();
        Task<CheckListDto> GetCheckListDetail(long id);
        Task UpdateCheckList(long id, CheckListDto checkListDto);
    }

    public class CheckListService : ICheckListService
    {
        private readonly IMapper mapper;
        private readonly IRepository<Domain.Registry.CheckList> checklistRepository;
        private readonly ILacosDbContext dbContext;

        public CheckListService(
            IMapper mapper,
            IRepository<Domain.Registry.CheckList> checklistRepository,
            ILacosDbContext dbContext)
        {
            this.mapper = mapper;
            this.checklistRepository = checklistRepository;
            this.dbContext = dbContext;
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
                throw new ApplicationException("Impossibile recuperare una checklist con id 0");

            var checklist = await checklistRepository
                .Query()
                .AsNoTracking()
                .Include(x=>x.ProductType)
                .Include(x=>x.ActivityType)
                .Include(x=>x.Items)
                .Where(x => x.Id == id)
                .SingleOrDefaultAsync();

            if (checklist == null)
                throw new ApplicationException($"Impossibile trovare la checklist con id {id}");

            return checklist.MapTo<CheckListDto>(mapper);
        }

        public async Task UpdateCheckList(long id, CheckListDto checkListDto)
        {
            if (id == 0)
                throw new ApplicationException("Impossibile aggiornare una checklist con id 0");

            var checklist= await checklistRepository
                .Query()
                .Where(x => x.Id == id)
                .SingleOrDefaultAsync();

            if (checklist == null)
                throw new ApplicationException($"Impossibile trovare una quotation con id {id}");
            
            checkListDto.MapTo(checklist, mapper);
            await dbContext.SaveChanges();
        }



    }
}
