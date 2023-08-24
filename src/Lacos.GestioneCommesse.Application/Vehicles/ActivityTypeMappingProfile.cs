using AutoMapper;
using Lacos.GestioneCommesse.Application.Vehicles.DTOs;
using Lacos.GestioneCommesse.Domain.Registry;
using Lacos.GestioneCommesse.Framework.Extensions;

namespace Lacos.GestioneCommesse.Application.Vehicles
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
