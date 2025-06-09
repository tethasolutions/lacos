using Lacos.GestioneCommesse.Domain.Registry;

namespace Lacos.GestioneCommesse.Domain.Docs;

public class PurchaseOrder : FullAuditedEntity
{
    public int Number { get; set; }
    public int Year { get; set; }
    public DateTimeOffset Date { get; set; }
    public DateTimeOffset? ExpectedDate { get; set; }
    public string? Description { get; set; }
    public PurchaseOrderStatus Status { get; set; }

    public long? ActivityTypeId { get; set; }
    public ActivityType ActivityType { get; set; }

    public ICollection<Job> Jobs { get; set; }

    public long SupplierId { get; set; }
    public Supplier? Supplier { get; set; }

    public long? OperatorId { get; set; }
    public Operator? Operator { get; set; }


    public ICollection<PurchaseOrderItem> Items { get; set; }
    public ICollection<PurchaseOrderAttachment> Attachments { get; set; }
    public ICollection<Message> Messages { get; set; }
    public ICollection<Activity> ParentActivities { get; set; }

    public PurchaseOrder()
    {
        Jobs = new List<Job>();
        Items = new List<PurchaseOrderItem>();
        Attachments = new List<PurchaseOrderAttachment>();
        Messages = new List<Message>();
        ParentActivities = new List<Activity>();
    }

    public void SetCode(int year, int number)
    {
        Year = year;
        Number = number;
    }
}
