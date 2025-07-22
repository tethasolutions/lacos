using Lacos.GestioneCommesse.Domain.Docs;

namespace Lacos.GestioneCommesse.Domain.Registry;

public class Vehicle : FullAuditedEntity, ILogEntity
{
    public string? Name { get; set; }
    public string? Plate { get; set; }
    public string? Notes { get; set; }
    
    public ICollection<Operator> Operators { get; set; }
    public ICollection<Intervention> Interventions { get; set; }

    public Vehicle()
    {
        Operators = new List<Operator>();
        Interventions = new List<Intervention>();
    }
}