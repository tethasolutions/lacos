using AutoMapper;
using Lacos.GestioneCommesse.Application.Docs.DTOs;
using Lacos.GestioneCommesse.Dal;
using Lacos.GestioneCommesse.Domain.Docs;
using Lacos.GestioneCommesse.Framework.Extensions;

namespace Lacos.GestioneCommesse.Application.Docs;

public class PurchaseOrderMappingProfile : Profile
{
    public PurchaseOrderMappingProfile()
    {
        CreateMap<PurchaseOrder, PurchaseOrderReadModel>()
            .MapMember(x => x.Code, y => CustomDbFunctions.FormatCode(y.Number, y.Year, 3))
            .MapMember(x => x.SupplierName, y => y.Supplier!.Name)
            .MapMember(x => x.JobId, y => y.Jobs.Min(j => j.Id))
            .MapMember(x => x.Jobs, y => y.Jobs.Select(e => e.Id))
            .MapMember(x => x.JobCodes, y => string.Join(", ", y.Jobs.Select(job => CustomDbFunctions.FormatCode(job.Number, job.Year, 3))))
            .MapMember(x => x.OperatorName, y => y.Operator!.Name)
            .MapMember(x => x.HasAttachments, y => y.Attachments.Any())
            .MapMember(x => x.UnreadMessages, y => y.Messages.SelectMany(e => e.MessageNotifications).Count(e => !e.IsRead))
            .MapMember(x => x.Type, y => y.ActivityType.Name);

        CreateMap<PurchaseOrderDto, PurchaseOrder>()
            .IgnoreCommonMembers()
            .Ignore(x => x.Supplier)
            .Ignore(x => x.Jobs)
            .Ignore(x => x.Items)
            .Ignore(x => x.Attachments)
            .Ignore(x => x.Operator)
            .Ignore(x => x.Messages)
            .Ignore(x => x.ActivityType)
            .Ignore(x => x.ParentActivities)
            .AfterMap(AfterMap);

        CreateMap<PurchaseOrder, PurchaseOrderDto>()
            .MapMember(x => x.Jobs, y => y.Jobs.Select(e => e.Id))
            .MapMember(x => x.SupplierName, y => y.Supplier!.Name);

        CreateMap<PurchaseOrderItemDto, PurchaseOrderItem>()
            .IgnoreCommonMembers()
            .Ignore(x => x.Product)
            .Ignore(x => x.PurchaseOrder);

        CreateMap<PurchaseOrderItem, PurchaseOrderItemDto>()
            .MapMember(x => x.ProductName, y => y.Product!.Code + " - " + y.Product!.Name)
            .MapMember(x => x.ProductImage, y => y.Product!.PictureFileName);

        CreateMap<PurchaseOrderAttachment, PurchaseOrderAttachmentReadModel>();
        CreateMap<PurchaseOrderAttachmentReadModel, PurchaseOrderAttachment>()
           .Ignore(x => x.PurchaseOrder)
           .Ignore(x => x.PurchaseOrderId)
           .IgnoreCommonMembers();

        CreateMap<PurchaseOrderAttachment, PurchaseOrderAttachmentDto>();
        CreateMap<PurchaseOrderAttachmentDto, PurchaseOrderAttachment>()
           .Ignore(x => x.PurchaseOrder)
           .IgnoreCommonMembers();
    }

    private static void AfterMap(PurchaseOrderDto orderDto, PurchaseOrder order, ResolutionContext context)
    {
        orderDto.Items.Merge(order.Items, (itemDto, item) => itemDto.Id == item.Id, (_, item) => item.PurchaseOrderId = order.Id, context);
        orderDto.Attachments.Merge(order.Attachments, (itemDto, item) => itemDto.Id == item.Id, (_, item) => item.PurchaseOrderId = order.Id, context);
        orderDto.Messages.Merge(order.Messages, (itemDto, item) => itemDto.Id == item.Id, (_, item) => item.PurchaseOrderId = order.Id, context);
    }
}