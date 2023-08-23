using Lacos.GestioneCommesse.Domain.Docs;

namespace Lacos.GestioneCommesse.Application.Jobs.DTOs;

public class JobReadModel
{
    public long Id { get; set; }
    public string? Code { get; set; }
    public DateTimeOffset Date { get; set; }
    public string? Description { get; set; }
    public JobStatus Status { get; set; }
    public string? Customer { get; set; }
    public bool CanBeRemoved { get; set; }
}