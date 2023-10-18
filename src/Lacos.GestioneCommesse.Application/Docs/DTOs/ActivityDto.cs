using Lacos.GestioneCommesse.Domain.Docs;

namespace Lacos.GestioneCommesse.Application.Docs.DTOs;

public class ActivityDto
{
    public long Id { get; set; }
    public ActivityStatus Status { get; set; }
    public int? Number { get; set; }
    public string? Description { get; set; }
    public DateTimeOffset? ExpirationDate { get; set; }

    public long JobId { get; set; }
    public long CustomerAddressId { get; set; }
    public long TypeId { get; set; }
}