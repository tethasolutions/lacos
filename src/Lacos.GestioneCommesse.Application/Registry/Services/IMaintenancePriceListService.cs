using Lacos.GestioneCommesse.Dal;
using AutoMapper;
using Lacos.GestioneCommesse.Domain.Registry;
using Lacos.GestioneCommesse.Framework.Exceptions;
using Lacos.GestioneCommesse.Framework.Extensions;
using Microsoft.EntityFrameworkCore;
using Lacos.GestioneCommesse.Application.Registry.DTOs;

namespace Lacos.GestioneCommesse.Application.MaintenancePriceLists.Services
{

    public interface IMaintenancePriceListService
    {
        IQueryable<MaintenancePriceListDto> GetMaintenancePriceList();
        Task<MaintenancePriceListDto> GetMaintenancePriceListDetail(long id);
        Task UpdateMaintenancePriceList(long id, MaintenancePriceListDto maintenancePriceListDto);
        Task<MaintenancePriceListDto> CreateMaintenancePriceList(MaintenancePriceListDto maintenancePriceListDto);
        Task DeleteMaintenancePriceList(long id);
        Task<IEnumerable<MaintenancePriceListItemDto>> GetMaintenancePriceListItems(long maintenancePricelistId);
        Task<MaintenancePriceListItemDto> GetMaintenancePriceListItemDetail(long maintenancePriceListItemId);
        Task UpdateMaintenancePriceListItem(long id, MaintenancePriceListItemDto maintenancePriceListItemDto);
        Task DeleteMaintenancePriceListItem(long id);
        Task<MaintenancePriceListItemDto> CreateMaintenancePriceListItem(MaintenancePriceListItemDto maintenancePriceListItemDto);
    }

    public class MaintenancePriceListService : IMaintenancePriceListService
    {
        private readonly IMapper mapper;
        private readonly IRepository<Domain.Registry.MaintenancePriceList> maintenancePricelistRepository;
        private readonly IRepository<MaintenancePriceListItem> maintenancePricelistItemRepository;
        private readonly ILacosDbContext dbContext;


        public MaintenancePriceListService(
            IMapper mapper,
            IRepository<Domain.Registry.MaintenancePriceList> maintenancePricelistRepository,
            ILacosDbContext dbContext, IRepository<MaintenancePriceListItem> maintenancePricelistItemRepository)
        {
            this.mapper = mapper;
            this.maintenancePricelistRepository = maintenancePricelistRepository;
            this.dbContext = dbContext;
            this.maintenancePricelistItemRepository = maintenancePricelistItemRepository;
        }

        public IQueryable<MaintenancePriceListDto> GetMaintenancePriceList()
        {
            var maintenancePricelists = maintenancePricelistRepository
                .Query()
                .AsNoTracking()
                .Project<MaintenancePriceListDto>(mapper);

            return maintenancePricelists;
        }

        public async Task<MaintenancePriceListDto> GetMaintenancePriceListDetail(long id)
        {
            if (id == 0)
                throw new LacosException("Impossibile recuperare una maintenancePricelist con id 0");

            var maintenancePricelist = await maintenancePricelistRepository
                .Query()
                .AsNoTracking()
                .Include(x => x.Items)
                .Where(x => x.Id == id)
                .SingleOrDefaultAsync();

            if (maintenancePricelist == null)
                throw new LacosException($"Impossibile trovare la maintenancePricelist con id {id}");

            return maintenancePricelist.MapTo<MaintenancePriceListDto>(mapper);
        }

        public async Task UpdateMaintenancePriceList(long id, MaintenancePriceListDto maintenancePriceListDto)
        {
            if (id == 0)
                throw new LacosException("Impossibile aggiornare una maintenancePricelist con id 0");

            var maintenancePricelist = await maintenancePricelistRepository
                .Query()
                .Where(x => x.Id == id)
                .Include(x => x.Items)
                .SingleOrDefaultAsync();

            if (maintenancePricelist == null)
                throw new LacosException($"Impossibile trovare una maintenancePricelist con id {id}");

            maintenancePriceListDto.MapTo(maintenancePricelist, mapper);
            await dbContext.SaveChanges();
        }

        public async Task<MaintenancePriceListDto> CreateMaintenancePriceList(MaintenancePriceListDto maintenancePriceListDto)
        {
            var maintenancePricelist = maintenancePriceListDto.MapTo<Domain.Registry.MaintenancePriceList>(mapper);
            await maintenancePricelistRepository.Insert(maintenancePricelist);
            
            try
            {
                await dbContext.SaveChanges();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            return maintenancePricelist.MapTo<MaintenancePriceListDto>(mapper);
        }

        public async Task DeleteMaintenancePriceList(long id)
        {
            if (id == 0)
                throw new LacosException("Impossible eliminare una maintenancePricelist con id 0");

            var maintenancePricelist = await maintenancePricelistRepository
                .Query()
                .Where(x => x.Id == id)
                .SingleOrDefaultAsync();

            if (maintenancePricelist == null)
                throw new LacosException($"Impossibile trovare una maintenancePricelist con id {id}");

            maintenancePricelistRepository.Delete(maintenancePricelist);
          
            await dbContext.SaveChanges();
        }

        public async Task<IEnumerable<MaintenancePriceListItemDto>> GetMaintenancePriceListItems(long maintenancePricelistId)
        {
            var maintenancePriceListItems = await maintenancePricelistItemRepository
                .Query()
                .AsNoTracking()
                .Where(x => x.MaintenancePriceListId == maintenancePricelistId)
                .ToListAsync();

            return maintenancePriceListItems.MapTo<IEnumerable<MaintenancePriceListItemDto>>(mapper);
        }

        public async Task<MaintenancePriceListItemDto> GetMaintenancePriceListItemDetail(long maintenancePriceListItemId)
        {
            if (maintenancePriceListItemId == 0)
                throw new LacosException("Impossibile recuperare un item con id 0");

            var maintenancePriceListItem = await maintenancePricelistItemRepository
                .Query()
                .AsNoTracking()
                
                .Where(x => x.Id == maintenancePriceListItemId)
                .SingleOrDefaultAsync();

            if (maintenancePriceListItem == null)
                throw new LacosException($"Impossibile trovare un item con id {maintenancePriceListItemId}");

            return maintenancePriceListItem.MapTo<MaintenancePriceListItemDto>(mapper);

        }
        public async Task UpdateMaintenancePriceListItem(long id, MaintenancePriceListItemDto maintenancePriceListItemDto)
        {
            if (id == 0)
                throw new LacosException("Impossibile aggiornare un item con id 0");

            var maintenancePriceListItem = await maintenancePricelistItemRepository
                .Query()
                .Where(x => x.Id == id)
                .SingleOrDefaultAsync();

            if (maintenancePriceListItem == null)
                throw new LacosException($"Impossibile trovare un item  con id {id}");

            maintenancePriceListItemDto.MapTo(maintenancePriceListItem, mapper);
            await dbContext.SaveChanges();
        }

        public async Task<MaintenancePriceListItemDto> CreateMaintenancePriceListItem(MaintenancePriceListItemDto maintenancePriceListItemDto)
        {
            var maintenancePriceListItem = maintenancePriceListItemDto.MapTo<MaintenancePriceListItem>(mapper);
            await maintenancePricelistItemRepository.Insert(maintenancePriceListItem);
            
            try
            {
                await dbContext.SaveChanges();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            return maintenancePriceListItem.MapTo<MaintenancePriceListItemDto>(mapper);
        }

        public async Task DeleteMaintenancePriceListItem(long id)
        {
            if (id == 0)
                throw new LacosException("Impossible eliminare un item con id 0");

            var maintenancePriceListItem = await maintenancePricelistItemRepository
                .Query()
                .Where(x => x.Id == id)
                .SingleOrDefaultAsync();

            if (maintenancePriceListItem == null)
                throw new LacosException($"Impossibile trovare un item con id {id}");

            maintenancePricelistItemRepository.Delete(maintenancePriceListItem);
          
            await dbContext.SaveChanges();
        }

    }
}
