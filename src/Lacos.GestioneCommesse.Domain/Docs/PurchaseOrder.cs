using Lacos.GestioneCommesse.Domain.Registry;

namespace Lacos.GestioneCommesse.Domain.Docs;

public class PurchaseOrder : FullAuditedEntity
{
    public string? Description { get; set; }
    public PurchaseOrderStatus Status { get; set; }

    public long? JobId { get; set; }
    public Job? Job { get; set; }

    public long SupplierId { get; set; }
    public Supplier? Supplier { get; set; }

    public ICollection<PurchaseOrderItem> Items { get; set; }

    public Activity? GeneratedActivity { get; set; }

    public PurchaseOrder()
    {
        Items = new List<PurchaseOrderItem>();
    }
}