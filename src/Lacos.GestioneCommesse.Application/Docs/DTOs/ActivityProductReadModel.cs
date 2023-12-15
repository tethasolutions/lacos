namespace Lacos.GestioneCommesse.Application.Docs.DTOs;

public class ActivityProductReadModel
{
    public long Id { get; set; }
    public string? Type { get; set; }
    public string? Code { get; set; }
    public string? Name { get; set; }
    public string? PictureFileName { get; set; }
    public string? QrCode { get; set; }
    public long ActivityId { get; set; }
    public string? Description { get; set; }
    public string? Location { get; set; }
    public bool CanBeRemoved { get; set; }
}