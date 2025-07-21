using Lacos.GestioneCommesse.Application.Docs.DTOs;
using Lacos.GestioneCommesse.Domain.Docs;

namespace Lacos.GestioneCommesse.Application.Docs.Services;

public interface IPurchaseOrdersService
{
    IQueryable<PurchaseOrderSummary> Query(long? jobId);
    Task<PurchaseOrderDto> Get(long id);
    Task<PurchaseOrderDto> Create(PurchaseOrderDto purchaseOrderDto);
    Task<PurchaseOrderDto> Update(PurchaseOrderDto purchaseOrderDto);
    Task<PurchaseOrderDto> CopyPurchaseOrder(CopyDto copyDto);
    Task Delete(long id);
    Task<int> GetNextNumber(int year);
    IQueryable<PurchaseOrderSummary> GetJobPurchaseOrders(long jobId);

    //attachments ----------------------------------------------------
    Task<IEnumerable<PurchaseOrderAttachmentReadModel>> GetPurchaseOrderAttachments(long jobId, long purchaseOrderId);
    Task<PurchaseOrderAttachmentReadModel> GetPurchaseOrderAttachmentDetail(long attachmentId);
    Task<PurchaseOrderAttachmentReadModel> DownloadPurchaseOrderAttachment(string filename);
    Task<PurchaseOrderAttachmentDto> UpdatePurchaseOrderAttachment(long id, PurchaseOrderAttachmentDto attachmentDto);
    Task<PurchaseOrderAttachmentDto> CreatePurchaseOrderAttachment(PurchaseOrderAttachmentDto attachmentDto);
}