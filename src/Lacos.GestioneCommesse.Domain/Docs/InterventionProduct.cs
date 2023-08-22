using Lacos.GestioneCommesse.Domain.Registry;

namespace Lacos.GestioneCommesse.Domain.Docs;

public class InterventionProduct : FullAuditedEntity
{
    public long ProductId { get; set; }
    public Product? Product { get; set; }
    public long ActivityId { get; set; }
    public Activity? Activity { get; set; }
    public long? InterventionId { get; set; }
    public Intervention? Intervention { get; set; }

    public InterventionProductCheckList? CheckList { get; set; }

    public ICollection<InterventionProductPicture> Pictures { get; set; }

    public InterventionProduct()
    {
        Pictures = new List<InterventionProductPicture>();
    }
}