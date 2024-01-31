using Lacos.GestioneCommesse.Domain.Registry;

namespace Lacos.GestioneCommesse.Domain.Docs;

public class Activity : FullAuditedEntity
{
    public DateTimeOffset? StartDate { get; set; }
    public DateTimeOffset? ExpirationDate { get; set; }

    public int RowNumber { get; set; }

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

    public ActivityStatus Status { get; set; }

    public ICollection<Intervention> Interventions { get; set; }
    public ICollection<ActivityProduct> ActivityProducts { get; set; }
    public ICollection<ActivityAttachment> Attachments { get; set; }

    public Activity()
    {
        Interventions = new List<Intervention>();
        ActivityProducts = new List<ActivityProduct>();
        Attachments = new List<ActivityAttachment>();
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
    Completed
}