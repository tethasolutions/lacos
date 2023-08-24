using AutoMapper;
using Lacos.GestioneCommesse.Application.CheckLists.DTOs;
using Lacos.GestioneCommesse.Domain.Registry;
using Lacos.GestioneCommesse.Framework.Extensions;

namespace Lacos.GestioneCommesse.Application.CheckLists
{

    public class ChecklistMappingProfile : Profile
    {
        public ChecklistMappingProfile()
        {

            CreateMap<CheckList, CheckListDto>();
            CreateMap<CheckListDto, CheckList>()    
                .Ignore(x => x.ProductType)
                .Ignore(x => x.ActivityType)
                .IgnoreCommonMembers();

            CreateMap<CheckListItem, CheckListItemDto>();
            CreateMap<CheckListItemDto, CheckListItem>()
                .Ignore(x => x.CheckList)
                .IgnoreCommonMembers();

        }
    }
}
