using Lacos.GestioneCommesse.Domain.Registry;

namespace Lacos.GestioneCommesse.Domain.Docs;

public class PurchaseOrderAttachment : FullAuditedEntity
{
    public string? DisplayName { get; set; }
    public string? FileName { get; set; }

    public long PurchaseOrderId { get; set; }
    public PurchaseOrder? PurchaseOrder { get; set; }
}