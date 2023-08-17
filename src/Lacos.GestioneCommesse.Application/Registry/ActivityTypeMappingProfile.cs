using AutoMapper;
using Lacos.GestioneCommesse.Application.Registry.DTOs;
using Lacos.GestioneCommesse.Domain.Registry;
using Lacos.GestioneCommesse.Framework.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                .Ignore(x => x.CheckLists);
        }
    }
}
