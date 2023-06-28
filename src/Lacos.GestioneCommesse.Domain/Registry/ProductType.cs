namespace Lacos.GestioneCommesse.Domain.Registry;

public class ProductType : FullAuditedEntity
{
    public string? Code { get; set; }
    public string? Name { get; set; }
    public bool IsReiDoor { get; set; }

    public ICollection<Product> Products { get; set; }
    public ICollection<CheckListItem> CheckList { get; set; }

    public ProductType()
    {
        Products = new List<Product>();
        CheckList = new List<CheckListItem>();
    }
}