using Lacos.GestioneCommesse.Domain.Security;

namespace Lacos.GestioneCommesse.Domain.Registry;

public class Vehicle : FullAuditedEntity
{
    public string? Name { get; set; }
    public string? Plate { get; set; }
    public string? Notes { get; set; }

    // assicurazione?
    // manutenzione?
    // giornale conducente?
    
    public ICollection<User> Users { get; set; }

    public Vehicle()
    {
        Users = new List<User>();
    }
}