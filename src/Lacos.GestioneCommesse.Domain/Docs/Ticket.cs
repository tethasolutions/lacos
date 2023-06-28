using Lacos.GestioneCommesse.Domain.Registry;

namespace Lacos.GestioneCommesse.Domain.Docs;

public class Ticket : FullAuditedEntity
{
    public string? Description { get; set; }
    public TicketStatus Status { get; set; }

    public long? InterventionId { get; set; }
    public Intervention? Intervention { get; set; }

    public long CustomerId { get; set; }
    public Customer? Customer { get; set; }

    public long CustomerAddressId { get; set; }
    public CustomerAddress? CustomerAddress { get; set; }

    public Activity? GeneratedActivity { get; set; }

    public ICollection<TicketPicture> Pictures { get; set; }

    public Ticket()
    {
        Pictures = new List<TicketPicture>();
    }
}