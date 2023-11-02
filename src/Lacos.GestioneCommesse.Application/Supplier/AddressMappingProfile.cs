using AutoMapper;
using Lacos.GestioneCommesse.Application.Suppliers.DTOs;
using Lacos.GestioneCommesse.Domain.Registry;
using Lacos.GestioneCommesse.Framework.Extensions;

namespace Lacos.GestioneCommesse.Application.Suppliers
{
    internal class AddressMappingProfile : Profile
    {
        public AddressMappingProfile()
        {
            CreateMap<SupplierAddress, AddressDto>();

            CreateMap<AddressDto, SupplierAddress>()
                .IgnoreCommonMembers()
                .Ignore(x => x.Notes)
                .Ignore(x => x.Supplier);
        }
    }
}
