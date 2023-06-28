using Lacos.GestioneCommesse.Domain.Security;

namespace Lacos.GestioneCommesse.Domain.Registry;

public class Vehicle : FullAuditedEntity
{
    public string? Name { get; set; }
    public string? Plate { get; set; }
    public string? Notes { get; set; }
    
    public ICollection<Operator> Operators { get; set; }

    public Vehicle()
    {
        Operators = new List<Operator>();
    }
}