using Lacos.GestioneCommesse.Domain.Docs;

namespace Lacos.GestioneCommesse.Domain.Registry;

public class Product : FullAuditedEntity
{
    public string? Code { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }

    // immagine della porta rei
    public string? PictureFileName { get; set; }

    // qrcode della porta rei
    public string? QrCode { get; set; }

    // associazione della porta rei con il cliente
    public long? CustomerId { get; set; }
    public Customer? Customer { get; set; }

    // associazione della porta rei con indirizzo del cliente
    public long? CustomerAddressId { get; set; }
    public CustomerAddress? CustomerAddress { get; set; }

    public long ProductTypeId { get; set; }
    public ProductType? ProductType { get; set; }

    public ICollection<PurchaseOrderItem> PurchaseOrderItems { get; set; }
    public ICollection<InterventionProduct> InterventionProducts { get; set; }
    public ICollection<ProductDocument> Documents { get; set; }

    public Product()
    {
        PurchaseOrderItems = new List<PurchaseOrderItem>();
        InterventionProducts = new List<InterventionProduct>();
        Documents = new List<ProductDocument>();
    }
}