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
}