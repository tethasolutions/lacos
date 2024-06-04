using Lacos.GestioneCommesse.Domain.Registry;

namespace Lacos.GestioneCommesse.Domain.Docs;

public class InterventionProductCheckListItemKO
{
    public long JobId { get; set; }
    public long ActivityId { get; set; }
    public string? JobCode { get; set; }
    public string? Customer { get; set;}
    public string? ActivityType { get; set;}
    public string? ActivityTypeColor { get; set; }
    public ActivityStatus ActivityStatus { get; set; }
    public string? ProductCode { get; set;}
    public string? ProductName { get; set;}
    public string? ProductDescription { get; set;}
    public string? ProductLocation { get; set; }
    public string? ChecklistItem { get; set; }
    public DateTimeOffset? Start { get; set; }
    public string? ShortDescription { get; set; }
    public string? InterventionDescription { get; set; }
    public string? OutcomeNotes { get; set; }
    public string? AttachmentFileName { get; set; }

}