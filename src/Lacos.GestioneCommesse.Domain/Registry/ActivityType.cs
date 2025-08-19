using Lacos.GestioneCommesse.Domain.Docs;

namespace Lacos.GestioneCommesse.Domain.Registry;

public class ActivityType : FullAuditedEntity, ILogEntity
{
    public string? Name { get; set; }

    public bool PictureRequired { get; set; }
    public bool IsInternal { get; set; }
    public bool IsExternal { get; set; }
    public bool HasQuotation { get; set; }
    public string? ColorHex { get; set; }

    public string? StatusLabel0 { get; set; }
    public string? StatusLabel1 { get; set; }
    public string? StatusLabel2 { get; set; }
    public string? StatusLabel3 { get; set; }

    public bool? InfluenceJobStatus { get; set; }
    public bool? ViewInPurchaseOrder { get; set; }
    public bool? HasDependencies { get; set; }

    public ICollection<Activity> Activities { get; set; }
    public ICollection<CheckList> CheckLists { get; set; }
    public ICollection<Operator> Operators { get; set; }

    public ActivityType()
    {
        Activities = new List<Activity>();
        CheckLists = new List<CheckList>();
        Operators = new List<Operator>();
    }
}