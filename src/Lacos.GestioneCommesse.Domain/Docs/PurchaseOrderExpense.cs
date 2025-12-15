using Lacos.GestioneCommesse.Domain.Registry;

namespace Lacos.GestioneCommesse.Domain.Docs;

public class PurchaseOrderExpense : FullAuditedEntity, ILogEntity
{
    public long PurchaseOrderId { get; set; }
    public PurchaseOrder? PurchaseOrder { get; set; }

    public long JobId { get; set; }
    public Job? Job { get; set; }

    public string? Note { get; set; }
    public decimal Quantity { get; set; }
    public decimal Amount { get; set; }

}