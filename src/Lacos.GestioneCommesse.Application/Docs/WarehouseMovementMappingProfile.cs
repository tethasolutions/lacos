using AutoMapper;
using Lacos.GestioneCommesse.Application.Docs.DTOs;
using Lacos.GestioneCommesse.Domain.Docs;
using Lacos.GestioneCommesse.Framework.Extensions;

namespace Lacos.GestioneCommesse.Application.Docs;

internal class WarehouseMovementMappingProfile : Profile
{
    public WarehouseMovementMappingProfile()
    {
        CreateMap<WarehouseMovement, WarehouseMovementDto>();

        CreateMap<WarehouseMovementDto, WarehouseMovement>()
            .IgnoreCommonMembers()
            .Ignore(x => x.Product);

        CreateMap<WarehouseMovement, WarehouseMovementReadModel>()
            .MapMember(x => x.ProductCode, y => y.Product != null ? y.Product.Code : null)
            .MapMember(x => x.ProductName, y => y.Product != null ? y.Product.Name : null);
    }
}
