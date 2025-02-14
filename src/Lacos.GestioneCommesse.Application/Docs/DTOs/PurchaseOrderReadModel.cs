using Lacos.GestioneCommesse.Application.Shared;
using Lacos.GestioneCommesse.Domain.Docs;
using Lacos.GestioneCommesse.Domain.Registry;

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
    public string? JobCode { get; set; }
    public string? JobReference { get; set; }
    public bool? JobHasHighPriority { get; set; }

    public long? CustomerId { get; set; }
    public string? CustomerName { get; set; }

    public string? SupplierName { get; set; }
    public string? OperatorName { get; set; }

    public int? UnreadMessages { get; set; }

}