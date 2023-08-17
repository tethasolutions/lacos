using AutoMapper;
using Lacos.GestioneCommesse.Application.Operators.DTOs;
using Lacos.GestioneCommesse.Domain.Registry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lacos.GestioneCommesse.Application.Products.DTOs;
using Lacos.GestioneCommesse.Framework.Extensions;

namespace Lacos.GestioneCommesse.Application.Products
{
    public class ProductMappingProfile: Profile
    {
        public ProductMappingProfile()
        {
            CreateMap<Product, ProductDto>();
            
            CreateMap<ProductDto, Product>()
                .IgnoreCommonMembers()
                .Ignore(x => x.Customer)
                .Ignore(x => x.CustomerAddress)
                .Ignore(x => x.ProductType)
                .Ignore(x => x.PurchaseOrderItems)
                .Ignore(x => x.InterventionProducts)
                .Ignore(x => x.Documents);

            CreateMap<Product, ProductReadModel>();

            CreateMap<ProductReadModel, Product>()
                .Ignore(x => x.Customer)
                .Ignore(x => x.CustomerId)
                .Ignore(x => x.CustomerAddress)
                .Ignore(x => x.CustomerAddressId)
                .Ignore(x => x.ProductType)
                .Ignore(x => x.ProductTypeId)
                .Ignore(x => x.PurchaseOrderItems)
                .Ignore(x => x.InterventionProducts)
                .Ignore(x => x.Documents)
                .IgnoreCommonMembers();
        }
    }
}
