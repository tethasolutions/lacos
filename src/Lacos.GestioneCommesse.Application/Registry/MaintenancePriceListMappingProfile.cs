using AutoMapper;
using Lacos.GestioneCommesse.Application.Customers.DTOs;
using Lacos.GestioneCommesse.Application.Registry.DTOs;
using Lacos.GestioneCommesse.Domain.Registry;
using Lacos.GestioneCommesse.Framework.Extensions;

namespace Lacos.GestioneCommesse.Application.CheckLists
{

    public class MaintenancePriceListMappingProfile : Profile
    {
        public MaintenancePriceListMappingProfile()
        {

            CreateMap<MaintenancePriceList, MaintenancePriceListDto>();
            CreateMap<MaintenancePriceListDto, MaintenancePriceList>()    
                .IgnoreCommonMembers()
                .AfterMap(AfterMap);

            CreateMap<MaintenancePriceListItem, MaintenancePriceListItemDto>();
            CreateMap<MaintenancePriceListItemDto, MaintenancePriceListItem>()
                .Ignore(x => x.MaintenancePriceList)
                .IgnoreCommonMembers();

        }
        private static void AfterMap(MaintenancePriceListDto maintenancePriceListDto, MaintenancePriceList maintenancePriceList, ResolutionContext context)
        {
            if (maintenancePriceListDto.Items != null) maintenancePriceListDto.Items.Merge(maintenancePriceList.Items, (priceListItemDto, priceListItem) => priceListItemDto.Id == priceListItem.Id, (_, item) => item.MaintenancePriceListId = maintenancePriceList.Id, context);
        }
    }
}
