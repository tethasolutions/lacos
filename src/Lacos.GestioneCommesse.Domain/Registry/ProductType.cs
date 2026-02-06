namespace Lacos.GestioneCommesse.Domain.Registry;

public class ProductType : FullAuditedEntity
{
    public string? Code { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public bool IsReiDoor { get; set; }
    public bool IsSparePart { get; set; }
    public string? ColorHex { get; set; }
    public bool IsWarehouseManaged { get; set; }

    public ICollection<Product> Products { get; set; }
    public ICollection<CheckList> CheckLists { get; set; }

    public ProductType()
    {
        Products = new List<Product>();
        CheckLists = new List<CheckList>();
    }
}