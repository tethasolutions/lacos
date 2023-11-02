using AutoMapper;
using Lacos.GestioneCommesse.Application.Suppliers.DTOs;
using Lacos.GestioneCommesse.Domain.Registry;
using Lacos.GestioneCommesse.Framework.Extensions;

namespace Lacos.GestioneCommesse.Application.Suppliers
{
    internal class SupplierMappingProfile : Profile
    {
        public SupplierMappingProfile()
        {
            CreateMap<Supplier, SupplierDto>();

            CreateMap<SupplierDto, Supplier>()
                .IgnoreCommonMembers()
                .Ignore(x => x.PurchaseOrders);
        }
    }
}
