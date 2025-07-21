using Lacos.GestioneCommesse.Domain.Registry;

namespace Lacos.GestioneCommesse.Domain.Docs;

public class PurchaseOrderSummary
{
    public long Id { get; set; }
    public string? JobCodes { get; set; }
    public string? JobIds { get; set; }
    public DateTimeOffset Date { get; set; }
    public DateTimeOffset? ExpectedDate { get; set; }
    public PurchaseOrderStatus Status { get; set; }
    public string? JobReferences { get; set; }
    public string? SupplierName { get; set; }
    public string? Description { get; set; }
    public string? ActivityTypeName { get; set; }
    public string? OperatorName { get; set; }
    public bool HasAttachments { get; set; }
    public int UnreadMessages { get; set; }

}
