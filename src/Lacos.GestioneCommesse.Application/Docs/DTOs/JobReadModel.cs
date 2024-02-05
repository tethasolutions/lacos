using Lacos.GestioneCommesse.Domain.Docs;

namespace Lacos.GestioneCommesse.Application.Docs.DTOs;

public class JobReadModel
{
    public long Id { get; set; }
    public string? Code { get; set; }
    public DateTimeOffset Date { get; set; }
    public string? Description { get; set; }
    public string? Reference { get; set; }
    public bool HasHighPriority { get; set; }
    public JobStatus Status { get; set; }
    public long CustomerId { get; set; }
    public string? Customer { get; set; }
    public string? CustomerContacts { get; set; }
    public long? AddressId { get; set; }
    public string? Address { get; set; }
    public bool CanBeRemoved { get; set; }
    public bool HasActivities { get; set; }
    public bool HasAttachments { get; set; }
    public bool HasPurchaseOrders { get; set; }
    public long? ReferentId { get; set; }
    public string? ReferentName { get; set; }
}