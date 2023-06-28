using Lacos.GestioneCommesse.Domain.Docs;

namespace Lacos.GestioneCommesse.Domain.Registry;

public class ActivityType : FullAuditedEntity
{
    public string? Name { get; set; }

    public ICollection<Activity> Activities { get; set; }
    public ICollection<CheckListItem> CheckList { get; set; }

    public ActivityType()
    {
        Activities = new List<Activity>();
        CheckList = new List<CheckListItem>();
    }
}