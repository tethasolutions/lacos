using Lacos.GestioneCommesse.Domain.Registry;

namespace Lacos.GestioneCommesse.Domain.Docs;

public class Job : FullAuditedEntity
{
    public JobStatus Status { get; set; }
    public string? Description { get; set; }

    public long CustomerId { get; set; }
    public Customer? Customer { get; set; }

    public ICollection<Activity> Activities { get; set; }

    public Job()
    {
        Activities = new List<Activity>();
    }
}