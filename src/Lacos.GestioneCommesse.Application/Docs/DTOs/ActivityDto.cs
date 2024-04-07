using Lacos.GestioneCommesse.Domain.Docs;

namespace Lacos.GestioneCommesse.Application.Docs.DTOs;

public class ActivityDto
{
    public long Id { get; set; }
    public ActivityStatus Status { get; set; }
    public int? Number { get; set; }
    public string? ShortDescription { get; set; }
    public string? Informations { get; set; }
    public string? Description { get; set; }
    public DateTimeOffset? StartDate { get; set; }
    public DateTimeOffset? ExpirationDate { get; set; }

    public long JobId { get; set; }
    public long? SupplierId { get; set; }
    public long? AddressId { get; set; }
    public long TypeId { get; set; }
    public long? ReferentId { get; set; }

    public string? StatusLabel0 { get; set; }
    public string? StatusLabel1 { get; set; }
    public string? StatusLabel2 { get; set; }

    public IEnumerable<ActivityAttachmentDto>? Attachments { get; set; }
    public IEnumerable<MessageReadModel>? Messages { get; set; }
}