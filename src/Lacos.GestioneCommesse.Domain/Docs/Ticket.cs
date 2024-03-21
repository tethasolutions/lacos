using Lacos.GestioneCommesse.Domain.Registry;

namespace Lacos.GestioneCommesse.Domain.Docs;

public class Ticket : FullAuditedEntity
{
    public int Number { get; set; }
    public int Year { get; set; }
    public DateTimeOffset TicketDate { get; set; }
    public string? Description { get; set; }
    public TicketStatus Status { get; set; }

    public long? JobId { get; set; }
    public Job? Job { get; set; }
    public long? ActivityId { get; set; }
    public Activity? Activity { get; set; }

    public long CustomerId { get; set; }
    public Customer? Customer { get; set; }
    public bool IsNew { get; set; }

    public long? OperatorId { get; set; }
    public Operator? Operator { get; set; }

    public ICollection<TicketPicture> Pictures { get; set; }

    public Ticket()
    {
        Pictures = new List<TicketPicture>();
    }

    public void SetCode(int year, int number)
    {
        Year = year;
        Number = number;
    }
}