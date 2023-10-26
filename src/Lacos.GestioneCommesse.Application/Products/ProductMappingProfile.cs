using AutoMapper;
using Lacos.GestioneCommesse.Domain.Registry;
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
                .Ignore(x => x.ActivityProducts)
                .Ignore(x => x.Documents);

            CreateMap<Product, ProductReadModel>()
                .MapMember(x => x.QrCode, y => (y.QrCodePrefix ?? "") + (y.QrCodeNumber?? "")) 
                .MapMember(x => x.ProductType, y => y.ProductType.Name);

            CreateMap<ProductReadModel, Product>()
                .Ignore(x => x.Customer)
                .Ignore(x => x.CustomerId)
                .Ignore(x => x.CustomerAddress)
                .Ignore(x => x.CustomerAddressId)
                .Ignore(x => x.ProductType)
                .Ignore(x => x.ProductTypeId)
                .Ignore(x => x.PurchaseOrderItems)
                .Ignore(x => x.ActivityProducts)
                .Ignore(x => x.Location)
                .Ignore(x => x.SerialNumber)
                .Ignore(x => x.ReiType)
                .Ignore(x => x.ConstructorName)
                .Ignore(x => x.HasPushBar)
                .Ignore(x => x.Year)
                .Ignore(x => x.VocType)
                .Ignore(x => x.NumberOfDoors)
                .Ignore(x => x.Documents)
                .Ignore(x => x.QrCodeNumber)
                .Ignore(x => x.QrCodePrefix)
                .IgnoreCommonMembers();

            CreateMap<ProductDocument, ProductDocumentReadModel>();
            CreateMap<ProductDocumentReadModel, ProductDocument>()
                .Ignore(x => x.Product)
                .Ignore(x => x.ProductId)
                .IgnoreCommonMembers();

            CreateMap<ProductDocument, ProductDocumentDto>();
            CreateMap<ProductDocumentDto, ProductDocument>()
                .Ignore(x => x.Product)
                .IgnoreCommonMembers();
        }
    }
}
