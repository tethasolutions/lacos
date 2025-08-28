using Lacos.GestioneCommesse.Domain.Docs;

namespace Lacos.GestioneCommesse.Application.Docs.DTOs;

public class ActivityDetailDto
{
    public long Id { get; set; }
    public ActivityStatus Status { get; set; }
    public int Number { get; set; }
    public string? ShortDescription { get; set; }
    public string? Informations { get; set; }
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
    public DateTimeOffset? StartDate { get; set; }
    public DateTimeOffset? ExpirationDate { get; set; }
    public bool? IsMandatoryExpiration { get; set; }
    public long? ReferentId { get; set; }
    public string? Referent { get; set; }

    public string? StatusLabel0 { get; set; }
    public string? StatusLabel1 { get; set; }
    public string? StatusLabel2 { get; set; }
    public string? StatusLabel3 { get; set; }

    public bool? IsFloorDelivery { get; set; }
    public bool? CanHaveDependencies { get; set; }
    public bool? HasDependencies { get; set; }
    public int? TotalDependencies { get; set; }
    public int? FulfilledDependencies { get; set; }

    public bool? HasUnpaidAccounts { get; set; }
    
    public IEnumerable<ActivityAttachmentDto>? Attachments { get; set; }
    public IEnumerable<MessageReadModel>? Messages { get; set; }
}