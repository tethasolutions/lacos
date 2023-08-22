namespace Lacos.GestioneCommesse.Domain.Registry;

public class ProductDocument : FullAuditedEntity
{
    public string? OriginalFilename { get; set; }
    public string? FileName { get; set; }
    public string? Description { get; set; }

    public long ProductId { get; set; }
    public Product? Product { get; set; }
}