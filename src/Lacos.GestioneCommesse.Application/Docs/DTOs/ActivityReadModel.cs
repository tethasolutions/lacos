using Lacos.GestioneCommesse.Domain.Docs;

namespace Lacos.GestioneCommesse.Application.Docs.DTOs;

public class ActivityReadModel
{
    public long Id { get; set; }
    public int Number { get; set; }
    public long JobId { get; set; }
    public string? JobCode { get; set; }
    public bool? JobHasHighPriority { get; set; }
    public string? Description { get; set; }
    public ActivityStatus Status { get; set; }
    public string? Address { get; set; }
    public string? Customer { get; set; }
    public long TypeId { get; set; }
    public string? Type { get; set; }
    public string? ActivityColor { get; set; }
    public string? Source { get; set; }
    public bool CanBeRemoved { get; set; }
    public DateTimeOffset? ExpirationDate { get; set; }
}