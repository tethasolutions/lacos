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
            .MapMember(x => x.Type, y => y.Product!.ProductType!.Name)
            .MapMember(x => x.Code, y => y.Product!.Code)
            .MapMember(x => x.Name, y => y.Product!.Name)
            .MapMember(x => x.PictureFileName, y => y.Product!.PictureFileName)
            .MapMember(x => x.QrCode, y => y.Product!.QrCodePrefix + y.Product!.QrCodeNumber.ToString().PadLeft(4,'0'))
            .MapMember(x => x.CanBeRemoved, y =>
                y.InterventionProducts
                    .All(e => e.Intervention!.Status == InterventionStatus.Scheduled)
            );

        CreateMap<ActivityProductDto, ActivityProduct>()
            .IgnoreCommonMembers()
            .Ignore(x => x.Product)
            .Ignore(x => x.Activity)
            .Ignore(x => x.InterventionProducts);
    }
}