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
