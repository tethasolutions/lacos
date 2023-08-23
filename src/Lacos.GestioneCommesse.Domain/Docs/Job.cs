using Lacos.GestioneCommesse.Domain.Registry;

namespace Lacos.GestioneCommesse.Domain.Docs;

public class Job : FullAuditedEntity
{
    public int Number { get; set; }
    public int Year { get; set; }
    public DateTimeOffset JobDate { get; set; }
    public string? Description { get; set; }
    public JobStatus Status { get; set; }

    public long CustomerId { get; set; }
    public Customer? Customer { get; set; }

    public ICollection<Activity> Activities { get; set; }

    public Job()
    {
        Activities = new List<Activity>();
    }

    public void Cancel()
    {
        Status = JobStatus.Canceled;

        foreach (var activity in Activities)
        {
            activity.Cancel();
        }
    }

    public void SetCode(int year, int number)
    {
        Year = year;
        Number = number;
    }

    public bool HasActivities()
    {
        return Activities.Any();
    }
}