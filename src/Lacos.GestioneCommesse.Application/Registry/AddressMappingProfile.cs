using AutoMapper;
using Lacos.GestioneCommesse.Application.Registry.DTOs;
using Lacos.GestioneCommesse.Domain.Registry;
using Lacos.GestioneCommesse.Framework.Extensions;

namespace Lacos.GestioneCommesse.Application.Registry
{
    internal class AddressMappingProfile : Profile
    {
        public AddressMappingProfile()
        {
            CreateMap<Address, AddressDto>();

            CreateMap<AddressDto, Address>()
                .IgnoreCommonMembers()
                .Ignore(x => x.Notes)
                .Ignore(x => x.Customer)
                .Ignore(x => x.Supplier)
                .Ignore(x => x.Jobs)
                .Ignore(x => x.Activities)
                .Ignore(x => x.Tickets);
            
        }
    }
}
