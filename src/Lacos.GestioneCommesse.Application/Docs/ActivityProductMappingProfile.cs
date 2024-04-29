using AutoMapper;
using Lacos.GestioneCommesse.Application.Docs.DTOs;
using Lacos.GestioneCommesse.Domain.Docs;
using Lacos.GestioneCommesse.Framework.Extensions;

namespace Lacos.GestioneCommesse.Application.Docs;

public class ActivityProductMappingProfile : Profile
{
    public ActivityProductMappingProfile()
    {
        CreateMap<ActivityProduct, ActivityProductReadModel>()
            .MapMember(x => x.Type, y => y.Product == null ? null : y.Product.ProductType!.Name)
            .MapMember(x => x.Code, y => y.Product == null ? null : y.Product.Code)
            .MapMember(x => x.Name, y => y.Product == null ? null : y.Product.Name)
            .MapMember(x => x.PictureFileName, y => y.Product == null ? null : y.Product.PictureFileName)
            .MapMember(x => x.QrCode, y => 
                y.Product == null || y.Product.QrCodeNumber == null 
                    ? null 
                    : y.Product.QrCodePrefix + y.Product.QrCodeNumber
            )
            .MapMember(x => x.CanBeRemoved, 
                y => y.InterventionProducts
                    .All(e => e.Intervention!.Status == InterventionStatus.Scheduled)
            )
            .MapMember(x => x.ColorHex, y => y.Product.ProductType.ColorHex);

        CreateMap<ActivityProductDto, ActivityProduct>()
            .IgnoreCommonMembers()
            .Ignore(x => x.Product)
            .Ignore(x => x.Activity)
            .Ignore(x => x.InterventionProducts);

        CreateMap<ActivityProduct, ActivityProductDto>()
            .MapMember(x => x.ProductTypeId , y => y.Product.ProductTypeId);
    }
}