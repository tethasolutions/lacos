using Lacos.GestioneCommesse.Domain.Docs;

namespace Lacos.GestioneCommesse.Application.Docs.DTOs;

public class ActivityDetailDto
{
    public long Id { get; set; }
    public ActivityStatus Status { get; set; }
    public int Number { get; set; }
    public string? Description { get; set; }
    public long JobId { get; set; }
    public string? Job { get; set; }
    public long CustomerId { get; set; }
    public string? Customer { get; set; }
    public long? SupplierId { get; set; }
    public long AddressId { get; set; }
    public string? Address { get; set; }
    public long TypeId { get; set; }
    public string? Type { get; set; }
    public string? AttachmentFileName { get; set; }
    public string? AttachmentDisplayName { get; set; }
    public string? Source { get; set; }
    public DateTimeOffset? ExpirationDate { get; set; }
}