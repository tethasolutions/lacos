using Lacos.GestioneCommesse.Application.Shared;
using Lacos.GestioneCommesse.Domain.Docs;
using Lacos.GestioneCommesse.Domain.Registry;

namespace Lacos.GestioneCommesse.Application.Docs.DTOs;

public class JobDto : BaseEntityDto
{
    public int? Number { get; set; }
    public int? Year { get; set; }
    public DateTimeOffset Date { get; set; }
    public DateTimeOffset? ExpirationDate { get; set; }
    public DateTimeOffset? MandatoryDate { get; set; }
    public string? Description { get; set; }
    public string? Reference { get; set; }
    public bool HasHighPriority { get; set; }
    public JobStatus Status { get; set; }

    public long CustomerId { get; set; }
    public long? AddressId { get; set; }
    public long? ReferentId { get; set; }
    public string? SharepointFolder { get; set; }
    public string? sharepointFolderName { get; set; }
    public IEnumerable<JobAttachmentDto>? Attachments { get; set; }
    public IEnumerable<MessageReadModel>? Messages { get; set; }
}