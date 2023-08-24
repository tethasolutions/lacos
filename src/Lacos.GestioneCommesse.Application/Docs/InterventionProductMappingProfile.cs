using AutoMapper;
using Lacos.GestioneCommesse.Application.Docs.DTOs;
using Lacos.GestioneCommesse.Domain.Docs;
using Lacos.GestioneCommesse.Framework.Extensions;

namespace Lacos.GestioneCommesse.Application.Docs;

public class InterventionProductMappingProfile : Profile
{
    public InterventionProductMappingProfile()
    {
        CreateMap<InterventionProduct, InterventionProductReadModel>()
            .MapMember(x => x.Type, y => y.Product!.ProductType!.Name)
            .MapMember(x => x.Code, y => y.Product!.Code)
            .MapMember(x => x.Name, y => y.Product!.Name)
            .MapMember(x => x.PictureFileName, y => y.Product!.PictureFileName)
            .MapMember(x => x.QrCode, y => y.Product!.QrCode)
            .MapMember(x => x.InterventionStart, y => y.Intervention == null ? (DateTimeOffset?) null : y.Intervention.Start)
            .MapMember(x => x.InterventionEnd, y => y.Intervention == null ? (DateTimeOffset?)null : y.Intervention.Start)
            .MapMember(x => x.InterventionOperators, y => y.Intervention == null ? null : y.Intervention.Operators.Select(e => e.Name))
            .MapMember(x => x.CanBeRemoved, y => y.Intervention == null || y.Intervention.Status == InterventionStatus.Pending);

        CreateMap<InterventionProductDto, InterventionProduct>()
            .IgnoreCommonMembers()
            .Ignore(x => x.Product)
            .Ignore(x => x.Activity)
            .Ignore(x => x.InterventionId)
            .Ignore(x => x.Intervention)
            .Ignore(x => x.CheckList)
            .Ignore(x => x.Pictures);
    }
}