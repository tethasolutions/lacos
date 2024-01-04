using AutoMapper;
using Lacos.GestioneCommesse.Application.Docs.DTOs;
using Lacos.GestioneCommesse.Domain.Docs;
using Lacos.GestioneCommesse.Framework.Extensions;

namespace Lacos.GestioneCommesse.Application.Docs;

public class PurchaseOrderMappingProfile : Profile
{
    public PurchaseOrderMappingProfile()
    {
        CreateMap<PurchaseOrder, PurchaseOrderReadModel>()
            .MapMember(x => x.Code, y => y.Year.ToString() + "/" + y.Number.ToString())
            .MapMember(x => x.SupplierName, y => y.Supplier!.Name)
            .MapMember(x => x.JobCode, y => y.Job!.Year.ToString() + "/" + y.Job.Number.ToString())
            .MapMember(x => x.JobHasHighPriority, y => y.Job!.HasHighPriority)
            .MapMember(x => x.JobReference, y => y.Job!.Reference);

        CreateMap<PurchaseOrderDto, PurchaseOrder>()
            .IgnoreCommonMembers()
            .Ignore(x => x.Job)
            .Ignore(x => x.Supplier)
            .Ignore(x => x.Items)
            .AfterMap(AfterMap);

        CreateMap<PurchaseOrder, PurchaseOrderDto>()
            .MapMember(x => x.SupplierName, y => y.Supplier!.Name);

        CreateMap<PurchaseOrderItemDto, PurchaseOrderItem>()
            .IgnoreCommonMembers()
            .Ignore(x => x.Product)
            .Ignore(x => x.PurchaseOrder);

        CreateMap<PurchaseOrderItem, PurchaseOrderItemDto>()
            .MapMember(x => x.ProductName, y => y.Product!.Code + " - " + y.Product!.Name)
            .MapMember(x => x.ProductImage, y => y.Product!.PictureFileName);
    }

    private static void AfterMap(PurchaseOrderDto orderDto, PurchaseOrder order, ResolutionContext context)
    {
        orderDto.Items.Merge(order.Items, (itemDto, item) => itemDto.Id == item.Id, (_, item) => item.PurchaseOrderId = order.Id, context);
    }
}