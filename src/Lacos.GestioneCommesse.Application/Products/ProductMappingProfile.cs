using AutoMapper;
using Lacos.GestioneCommesse.Domain.Registry;
using Lacos.GestioneCommesse.Application.Products.DTOs;
using Lacos.GestioneCommesse.Framework.Extensions;
using Lacos.GestioneCommesse.Application.Docs.DTOs;

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
                .Ignore(x => x.Address)
                .Ignore(x => x.ProductType)
                .Ignore(x => x.PurchaseOrderItems)
                .Ignore(x => x.ActivityProducts)
                .Ignore(x => x.Documents)
                .AfterMap(AfterMap);

            CreateMap<Product, ProductReadModel>()
                .MapMember(x => x.QrCode, y => (y.QrCodePrefix ?? "") + (y.QrCodeNumber?? "")) 
                .MapMember(x => x.ProductType, y => y.ProductType.Name);

            CreateMap<ProductReadModel, Product>()
                .Ignore(x => x.Note)
                .Ignore(x => x.Customer)
                .Ignore(x => x.CustomerId)
                .Ignore(x => x.Address)
                .Ignore(x => x.AddressId)
                .Ignore(x => x.ProductType)
                .Ignore(x => x.ProductTypeId)
                .Ignore(x => x.PurchaseOrderItems)
                .Ignore(x => x.ActivityProducts)
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
        private static void AfterMap(ProductDto productDto, Product product, ResolutionContext context)
        {
            if (productDto.Documents != null) productDto.Documents.Merge(product.Documents, (itemDto, item) => itemDto.Id == item.Id, (_, item) => item.ProductId = product.Id, context);
        }
    }
}
