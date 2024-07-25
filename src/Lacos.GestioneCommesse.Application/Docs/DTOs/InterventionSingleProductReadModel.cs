using Lacos.GestioneCommesse.Application.Registry.DTOs;
using Lacos.GestioneCommesse.Domain.Docs;

namespace Lacos.GestioneCommesse.Application.Docs.DTOs;

public class InterventionSingleProductReadModel
{
    public long Id { get; set; }
    public InterventionStatus Status { get; set; }
    public DateTimeOffset Start { get; set; }
    public DateTimeOffset End { get; set; }
    public string? Description { get; set; }
    public IEnumerable<string> Operators { get; set; }
    public long ActivityId { get; set; }
    public long JobId { get; set; }
    public long InterventionProductId { get; set; }

    public InterventionSingleProductReadModel()
    {
        Operators = new List<string>();
    }
}