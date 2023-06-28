using Lacos.GestioneCommesse.Domain.Registry;

namespace Lacos.GestioneCommesse.Domain.Docs;

public class PurchaseOrder : FullAuditedEntity
{
    public string? Description { get; set; }
    public PurchaseOrderStatus Status { get; set; }

    public long? InterventionId { get; set; }
    public Intervention? Intervention { get; set; }

    public long CustomerId { get; set; }
    public Customer? Customer { get; set; }

    public ICollection<PurchaseOrderItem> Items { get; set; }

    public Activity? GeneratedActivity { get; set; }

    public PurchaseOrder()
    {
        Items = new List<PurchaseOrderItem>();
    }
}