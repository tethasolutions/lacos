using Lacos.GestioneCommesse.Domain.Registry;

namespace Lacos.GestioneCommesse.Domain.Docs;

public class Activity : FullAuditedEntity
{
    public DateTimeOffset? ExpirationDate { get; set; }

    public int RowNumber { get; set; }

    public string? Description { get; set; }

    public long? CustomerAddressId { get; set; }
    public CustomerAddress? CustomerAddress { get; set; }

    public long JobId { get; set; }
    public Job? Job { get; set; }

    public long TypeId { get; set; }
    public ActivityType? Type { get; set; }

    public long? AddressId { get; set; }
    public Address? Address { get; set; }

    public long? SourceTicketId { get; set; }
    public Ticket? SourceTicket { get; set; }

    public long? SourcePuchaseOrderId { get; set; }
    public PurchaseOrder? SourcePurchaseOrder { get; set; }

    public ActivityStatus Status { get; set; }

    public ICollection<Intervention> Interventions { get; set; }
    public ICollection<ActivityProduct> ActivityProducts { get; set; }

    public Activity()
    {
        Interventions = new List<Intervention>();
        ActivityProducts = new List<ActivityProduct>();
    }

    public void SetNumber(int number)
    {
        RowNumber = number;
    }

    public bool HasCompletedInterventions()
    {
        return Interventions
            .Any(ee => ee.IsCompleted());
    }
}

public enum ActivityStatus
{
    Pending,
    InProgress,
    ReadyForCompletion,
    Completed
}