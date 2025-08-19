using Lacos.GestioneCommesse.Domain.Registry;

namespace Lacos.GestioneCommesse.Domain.Docs;

public class Activity : FullAuditedEntity, ILogEntity
{
    public DateTimeOffset? StartDate { get; set; }
    public DateTimeOffset? ExpirationDate { get; set; }
    public bool? IsMandatoryExpiration { get; set; }

    public int RowNumber { get; set; }

    public string? ShortDescription { get; set; }
    public string? Informations { get; set; }
    public string? Description { get; set; }

    public long JobId { get; set; }
    public Job? Job { get; set; }

    public long TypeId { get; set; }
    public ActivityType? Type { get; set; }

    public long? SupplierId { get; set; }
    public Supplier? Supplier { get; set; }

    public long? AddressId { get; set; }
    public Address? Address { get; set; }

    public long? ReferentId { get; set; }
    public Operator Referent { get; set; }
    public bool IsNewReferent { get; set; }

    public bool? IsFloorDelivery { get; set; }

    public ActivityStatus Status { get; set; }

    public decimal? QuotationAmount { get; set; }

    public ICollection<Intervention> Interventions { get; set; }
    public ICollection<ActivityProduct> ActivityProducts { get; set; }
    public ICollection<ActivityAttachment> Attachments { get; set; }
    public ICollection<Message> Messages { get; set; }
    public ICollection<Activity> ParentActivities{ get; set; }
    public ICollection<Activity> ActivityDependencies { get; set; }
    public ICollection<PurchaseOrder> PurchaseOrderDependencies { get; set; }

    public Activity()
    {
        Interventions = new List<Intervention>();
        ActivityProducts = new List<ActivityProduct>();
        Attachments = new List<ActivityAttachment>();
        Messages = new List<Message>();
        ActivityDependencies = new List<Activity>();
        PurchaseOrderDependencies = new List<PurchaseOrder>();
        ParentActivities = new List<Activity>();
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
    Ready,
    Completed
}