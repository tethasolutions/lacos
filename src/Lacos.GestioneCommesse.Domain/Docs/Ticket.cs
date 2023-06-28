namespace Lacos.GestioneCommesse.Domain.Docs;

public class Ticket : FullAuditedEntity
{
    public string? Notes { get; set; }
    public TicketStatus Status { get; set; }

    public long InterventionId { get; set; }
    public Intervention? Intervention { get; set; }

    public ICollection<Activity> GeneratedActivities { get; set; }
    public ICollection<TicketPicture> Pictures { get; set; }

    public Ticket()
    {
        GeneratedActivities = new List<Activity>();
        Pictures = new List<TicketPicture>();
    }
}