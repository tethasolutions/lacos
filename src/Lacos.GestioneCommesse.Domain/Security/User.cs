using Lacos.GestioneCommesse.Domain.Docs;
using Lacos.GestioneCommesse.Domain.Registry;

namespace Lacos.GestioneCommesse.Domain.Security;

public class User : BaseEntity
{
    public string? UserName { get; set; }
    public string? PasswordHash { get; set; }
    public string? Salt { get; set; }
    public string? AccessToken { get; set; }
    public bool Enabled { get; set; }
    public Role Role { get; set; }
    public string? EmailAddress { get; set; }
    public string? ColorHex { get; set; }
    public string? Name { get; set; }
    public string? Surname { get; set; }

    public long? VechicleId { get; set; }
    public Vehicle? Vehicle { get; set; }
    
    public ICollection<Activity> Activities { get; set; }
    public ICollection<OperatorDocument> OperatorDocuments { get; set; }
    public ICollection<Intervention> Interventions { get; set; }
    public ICollection<InterventionProductCheckListItem> InterventionProductCheckListItems { get; set; }
    public ICollection<InterventionDispute> InterventionDisputes { get; set; }

    public User()
    {
        Activities = new List<Activity>();
        OperatorDocuments = new List<OperatorDocument>();
        Interventions = new List<Intervention>();
        InterventionProductCheckListItems = new List<InterventionProductCheckListItem>();
        InterventionDisputes = new List<InterventionDispute>();
    }
}