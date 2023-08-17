using AutoMapper;
using Lacos.GestioneCommesse.Application.Vehicles.DTOs;
using Lacos.GestioneCommesse.Domain.Registry;
using Lacos.GestioneCommesse.Framework.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lacos.GestioneCommesse.Application.Registry
{
    internal class VehicleMappingProfile : Profile
    {
        public VehicleMappingProfile()
        {
            CreateMap<Vehicle, VehicleDto>();

            CreateMap<VehicleDto, Vehicle>()
                .IgnoreCommonMembers()
                .Ignore(x=> x.Operators)
                .Ignore(x => x.Interventions);
        }
    }
}
