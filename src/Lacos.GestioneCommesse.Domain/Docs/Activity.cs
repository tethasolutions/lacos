using Lacos.GestioneCommesse.Domain.Registry;

namespace Lacos.GestioneCommesse.Domain.Docs;

public class Activity : FullAuditedEntity
{
    public ActivityStatus Status { get; set; }

    public long CustomerAddressId { get; set; }
    public CustomerAddress? CustomerAddress { get; set; }

    public long JobId { get; set; }
    public Job? Job { get; set; }

    public long? TypeId { get; set; }
    public ActivityType? Type { get; set; }

    public long? SourceTicketId { get; set; }
    public Ticket? SourceTicket { get; set; }

    public long? SourcePuchaseOrderId { get; set; }
    public PurchaseOrder? SourcePurchaseOrder { get; set; }
}