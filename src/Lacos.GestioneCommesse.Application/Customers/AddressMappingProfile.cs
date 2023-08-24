using AutoMapper;
using Lacos.GestioneCommesse.Application.Customers.DTOs;
using Lacos.GestioneCommesse.Domain.Registry;
using Lacos.GestioneCommesse.Framework.Extensions;

namespace Lacos.GestioneCommesse.Application.Customers
{
    internal class AddressMappingProfile : Profile
    {
        public AddressMappingProfile()
        {
            CreateMap<CustomerAddress, AddressDto>();

            CreateMap<AddressDto, CustomerAddress>()
                .IgnoreCommonMembers()
                .Ignore(x => x.Notes)
                .Ignore(x => x.Customer)
                .Ignore(x => x.Products)
                .Ignore(x => x.Tickets)
                .Ignore(x => x.Activities);
        }
    }
}
