namespace Lacos.GestioneCommesse.Domain.Docs;

public class PurchaseOrder : FullAuditedEntity
{
    public string? Notes { get; set; }
    public PurchaseOrderStatus Status { get; set; }

    public long InterventionId { get; set; }
    public Intervention? Intervention { get; set; }

    public ICollection<PurchaseOrderItem> Items { get; set; }
    public ICollection<Activity> GeneratedActivities { get; set; }

    public PurchaseOrder()
    {
        Items = new List<PurchaseOrderItem>();
        GeneratedActivities = new List<Activity>();
    }
}