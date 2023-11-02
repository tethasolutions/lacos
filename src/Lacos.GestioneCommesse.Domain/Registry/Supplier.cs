using Lacos.GestioneCommesse.Domain.Docs;
using Lacos.GestioneCommesse.Domain.Security;

namespace Lacos.GestioneCommesse.Domain.Registry;

public class Supplier : FullAuditedEntity
{
    public string? Name { get; set; }
    public string? Notes { get; set; }
    public string? Telephone { get; set; }
    public string? Email { get; set; }

    public ICollection<SupplierAddress> Addresses { get; set; }
    public ICollection<PurchaseOrder> PurchaseOrders { get; set; }

    public Supplier()
    {
        Addresses = new List<SupplierAddress>();
        PurchaseOrders = new List<PurchaseOrder>();
    }
}