namespace Lacos.GestioneCommesse.Domain.Docs;

public class TicketPicture : FullAuditedEntity
{
    public string? FileName { get; set; }
    public string? Description { get; set; }

    public long TicketId { get; set; }
    public Ticket? Ticket { get; set; }
}