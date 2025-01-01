using Lacos.GestioneCommesse.Domain.Registry;

namespace Lacos.GestioneCommesse.Domain.Docs;

public class Job : FullAuditedEntity
{
    public int Number { get; set; }
    public int Year { get; set; }
    public DateTimeOffset JobDate { get; set; }
    public DateTimeOffset? ExpirationDate { get; set; }
    public string? Description { get; set; }
    public string? Reference { get; set; }
    public bool HasHighPriority { get; set; }

    public long CustomerId { get; set; }
    public Customer? Customer { get; set; }

    public long? AddressId { get; set; }
    public Address? Address { get; set; }

    public JobStatus Status { get; set; }
    
    //InternalJob for ticket issues
    public bool IsInternalJob { get; set; }
    public long? ReferentId { get; set; }
    public Operator Referent { get; set; }

    public string? SharepointFolder { get; set; }
    public string? sharepointFolderName { get; set; }

    public ICollection<Activity> Activities { get; set; }
    public ICollection<PurchaseOrder> PurchaseOrders { get; set; }
    public ICollection<Ticket> Tickets { get; set; }
    public ICollection<JobAttachment> Attachments { get; set; }
    public ICollection<Message> Messages { get; set; }

    public Job()
    {
        Activities = new List<Activity>();
        PurchaseOrders = new List<PurchaseOrder>();
        Attachments = new List<JobAttachment>();
        Messages = new List<Message>();
    }

    public void SetCode(int year, int number)
    {
        Year = year;
        Number = number;
    }

    public bool HasCompletedInterventions()
    {
        return Activities
            .Any(ee => ee.HasCompletedInterventions());
    }
}