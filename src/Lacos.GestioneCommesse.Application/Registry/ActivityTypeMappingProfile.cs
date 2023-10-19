using AutoMapper;
using Lacos.GestioneCommesse.Application.Registry.DTOs;
using Lacos.GestioneCommesse.Domain.Registry;
using Lacos.GestioneCommesse.Framework.Extensions;

namespace Lacos.GestioneCommesse.Application.Registry
{
    internal class ActivityTypeMappingProfile : Profile
    {
        public ActivityTypeMappingProfile()
        {
            CreateMap<ActivityType, ActivityTypeDto>();

            CreateMap<ActivityTypeDto, ActivityType>()
                .IgnoreCommonMembers()
                .Ignore(x=> x.Activities)
                .Ignore(x => x.CheckLists)
                .Ignore(x => x.Operators);
        }
    }
}
