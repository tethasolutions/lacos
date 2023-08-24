using AutoMapper;
using Lacos.GestioneCommesse.Application.Registry.DTOs;
using Lacos.GestioneCommesse.Domain.Registry;
using Lacos.GestioneCommesse.Framework.Extensions;

namespace Lacos.GestioneCommesse.Application.Registry
{
    internal class ProductTypeMappingProfile : Profile
    {
        public ProductTypeMappingProfile()
        {
            CreateMap<ProductType, ProductTypeDto>();

            CreateMap<ProductTypeDto, ProductType>()
                .IgnoreCommonMembers()
                .Ignore(x => x.Products)
                .Ignore(x => x.CheckLists);
        }
    }
}
