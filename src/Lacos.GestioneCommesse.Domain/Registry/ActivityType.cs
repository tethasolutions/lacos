using Lacos.GestioneCommesse.Domain.Docs;

namespace Lacos.GestioneCommesse.Domain.Registry;

public class ActivityType : FullAuditedEntity
{
    public string? Name { get; set; }

    public bool PictureRequired { get; set; }

    public ICollection<Activity> Activities { get; set; }
    public ICollection<CheckList> CheckLists { get; set; }

    public ActivityType()
    {
        Activities = new List<Activity>();
        CheckLists = new List<CheckList>();
    }
}