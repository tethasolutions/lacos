using Lacos.GestioneCommesse.Domain.Registry;

namespace Lacos.GestioneCommesse.Domain.Docs;

public class ActivityProduct : FullAuditedEntity, ILogEntity
{
    public long ProductId { get; set; }
    public Product? Product { get; set; }
    public string? Description { get; set; }
    public string? Location { get; set; }

    public long ActivityId { get; set; }
    public Activity? Activity { get; set; }

    public ICollection<InterventionProduct> InterventionProducts { get; set; }

    public ActivityProduct()
    {
        InterventionProducts = new List<InterventionProduct>();
    }

    public bool HasCompletedInterventions()
    {
        return InterventionProducts
            .Any(ee => ee.Intervention!.IsCompleted());
    }
}