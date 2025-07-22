using Lacos.GestioneCommesse.Domain.Registry;

namespace Lacos.GestioneCommesse.Domain.Docs;

public class Ticket : FullAuditedEntity, ILogEntity
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
    public long? PurchaseOrderId { get; set; }
    public PurchaseOrder? PurchaseOrder { get; set; }

    public long CustomerId { get; set; }
    public Customer? Customer { get; set; }

    public long? AddressId { get; set; }
    public Address? Address { get; set; }
    public bool IsNew { get; set; }

    public long? OperatorId { get; set; }
    public Operator? Operator { get; set; }

    public string? ReportFileName { get; set; }
    public DateTimeOffset? ReportGeneratedOn { get; set; }
    public string? CustomerSignatureName { get; set; }
    public string? CustomerSignatureFileName { get; set; }

    public ICollection<TicketPicture> Pictures { get; set; }
    public ICollection<Message> Messages { get; set; }

    public Ticket()
    {
        Pictures = new List<TicketPicture>();
        Messages = new List<Message>();
    }

    public void SetCode(int year, int number)
    {
        Year = year;
        Number = number;
    }
}