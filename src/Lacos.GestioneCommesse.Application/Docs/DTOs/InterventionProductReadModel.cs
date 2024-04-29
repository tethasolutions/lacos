namespace Lacos.GestioneCommesse.Application.Docs.DTOs;

public class InterventionProductReadModel
{
    public long InterventionProductId { get; set; }
    public string? Code { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? PictureFileName { get; set; }
    public string? QrCode { get; set; }
    public string? ProductType { get; set; }
    public string? ColorHex { get; set; }
}