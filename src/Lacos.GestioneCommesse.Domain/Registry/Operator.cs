using Lacos.GestioneCommesse.Domain.Docs;
using Lacos.GestioneCommesse.Domain.Security;

namespace Lacos.GestioneCommesse.Domain.Registry;

public class Operator : FullAuditedEntity
{
    public string? Email { get; set; }
    public string? ColorHex { get; set; }
    public string? Name { get; set; }

    public long? DefaultVehicleId { get; set; }
    public Vehicle? DefaultVehicle { get; set; }

    public long? UserId { get; set; }
    public User? User { get; set; }

    public ICollection<ActivityType> ActivityTypes { get; set; }

    public ICollection<OperatorDocument> Documents { get; set; }
    public ICollection<Intervention> Interventions { get; set; }
    public ICollection<InterventionProductPicture> InterventionProductPictures { get; set; }
    public ICollection<InterventionNote> InterventionNotes { get; set; }
    public ICollection<InterventionProductCheckListItem> InterventionProductCheckListItems { get; set; }

    public Operator()
    {
        ActivityTypes = new List<ActivityType>();
        Documents = new List<OperatorDocument>();
        Interventions = new List<Intervention>();
        InterventionProductPictures = new List<InterventionProductPicture>();
        InterventionProductCheckListItems = new List<InterventionProductCheckListItem>();
        InterventionNotes = new List<InterventionNote>();
    }
}