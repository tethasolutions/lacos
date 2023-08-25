using Lacos.GestioneCommesse.Domain.Docs;

namespace Lacos.GestioneCommesse.Application.Docs.DTOs;

public class InterventionDto
{
    public long Id { get; set; }
    public DateTimeOffset Start { get; set; }
    public DateTimeOffset End { get; set; }
    public InterventionStatus Status { get; set; }
    public string? Description { get; set; }
    public long? VehicleId { get; set; }
    public long ActivityId { get; set; }
    public long JobId { get; set; }
    public IEnumerable<long> Operators { get; set; }
    public IEnumerable<long> Products { get; set; }

    public InterventionDto()
    {
        Operators = new List<long>();
        Products = new List<long>();
    }
}