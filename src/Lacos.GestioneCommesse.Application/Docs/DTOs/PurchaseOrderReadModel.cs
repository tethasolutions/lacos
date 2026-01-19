using Lacos.GestioneCommesse.Domain.Docs;

namespace Lacos.GestioneCommesse.Application.Docs.DTOs;

public class PurchaseOrderReadModel
{
    public long Id { get; set; }
    public string? Code { get; set; }
    public DateTimeOffset Date { get; set; }
    public DateTimeOffset? ExpectedDate { get; set; }
    public string? Description { get; set; }
    public PurchaseOrderStatus Status { get; set; }

    public string? Type { get; set; }

    public bool HasAttachments { get; set; }

    public long? JobId { get; set; }
    public IEnumerable<long>? Jobs { get; set; }
    public string? JobCodes { get; set; }
    public string? JobReferences { get; set; }
    public string? SupplierName { get; set; }
    public string? OperatorName { get; set; }

    public int? UnreadMessages { get; set; }
    public decimal? TotalExpenses { get; set; }

}