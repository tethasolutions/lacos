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
    public string? QrCodePrefix { get; set; }
    public int? QrCodeNumber { get; set; }

    // associazione della porta rei con il cliente
    public long? CustomerId { get; set; }
    public Customer? Customer { get; set; }

    // associazione della porta rei con indirizzo del cliente
    public long? AddressId { get; set; }
    public Address? Address { get; set; }

    public long ProductTypeId { get; set; }
    public ProductType? ProductType { get; set; }

    public string? Location { get; set; }
    public string? SerialNumber { get; set; }
    public string? ReiType { get; set; }
    public string? ConstructorName { get; set; }
    public bool? HasPushBar { get; set; }
    public int? Year { get; set; }
    public string? VocType { get; set; }
    public int? NumberOfDoors { get; set; } //ante

    public ICollection<PurchaseOrderItem> PurchaseOrderItems { get; set; }
    public ICollection<ActivityProduct> ActivityProducts { get; set; }
    public ICollection<ProductDocument> Documents { get; set; }

    public Product()
    {
        PurchaseOrderItems = new List<PurchaseOrderItem>();
        ActivityProducts = new List<ActivityProduct>();
        Documents = new List<ProductDocument>();
    }
}