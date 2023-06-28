using Lacos.GestioneCommesse.Domain.Docs;
using Lacos.GestioneCommesse.Domain.Security;

namespace Lacos.GestioneCommesse.Domain.Registry;

public class Operator : FullAuditedEntity
{
    public string? Email { get; set; }
    public string? ColorHex { get; set; }
    public string? Name { get; set; }

    public long? DefaultVechicleId { get; set; }
    public Vehicle? DefaultVehicle { get; set; }

    public long UserId { get; set; }
    public User? User { get; set; }

    public ICollection<OperatorDocument> Documents { get; set; }
    public ICollection<Intervention> Interventions { get; set; }
    public ICollection<InterventionProductCheckListItem> InterventionProductCheckListItems { get; set; }

    public Operator()
    {
        Documents = new List<OperatorDocument>();
        Interventions = new List<Intervention>();
        InterventionProductCheckListItems = new List<InterventionProductCheckListItem>();
    }
}