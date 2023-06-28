using Lacos.GestioneCommesse.Domain.Registry;

namespace Lacos.GestioneCommesse.Domain.Docs;

public class InterventionProduct : FullAuditedEntity
{
    public long ProductId { get; set; }
    public Product? Product { get; set; }

    public long InterventionId { get; set; }
    public Intervention? Intervention { get; set; }

    public ICollection<InterventionProductCheckListItem> CheckList { get; set; }
    public ICollection<InterventionProductPicture> Pictures { get; set; }

    public InterventionProduct()
    {
        CheckList = new List<InterventionProductCheckListItem>();
        Pictures = new List<InterventionProductPicture>();
    }
}