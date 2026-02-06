namespace Lacos.GestioneCommesse.Application.Products.DTOs;

public class ProductReadModel
{
    public long Id { get; set; }
    public string? Code { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? Location { get; set; }
    public string? PictureFileName { get; set; }
    public string? QrCode { get; set; }
    public string? ProductType { get; set; }
    public long? AddressId { get; set; }
    public long ProductTypeId { get; set; }
    public bool? MonthlyMaintenance { get; set; }
    public bool? IsDecommissioned { get; set; }
    public string? SerialNumber { get; set; }
    public string? Brand { get; set; }
    public string? Side { get; set; }
    public string? Size { get; set; }
    public string? Material { get; set; }
}