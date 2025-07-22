using Lacos.GestioneCommesse.Domain.Registry;

namespace Lacos.GestioneCommesse.Domain.Docs;

public class Message : FullAuditedEntity, ILogEntity
{
    public DateTimeOffset Date { get; set; }

    public string Note { get; set; }

    public long OperatorId { get; set; }
    public Operator? Operator { get; set; }

    public long? JobId { get; set; }
    public Job? Job { get; set; }
    public long? ActivityId { get; set; }
    public Activity? Activity { get; set; }
    public long? TicketId { get; set; }
    public Ticket? Ticket { get; set; }
    public long? PurchaseOrderId { get; set; }
    public PurchaseOrder? PurchaseOrder { get; set; }
    public bool IsFromApp { get; set; }

    public ICollection<MessageNotification> MessageNotifications { get; set; }

    public Message()
    {
        MessageNotifications = new List<MessageNotification>();
    }
}