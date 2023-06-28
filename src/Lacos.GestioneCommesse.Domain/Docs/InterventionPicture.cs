namespace Lacos.GestioneCommesse.Domain.Docs;

public class InterventionNote : FullAuditedEntity
{
    public string? PictureFileName { get; set; }

    public string? Notes { get; set; }
    
    public long InterventionId { get; set; }
    public Intervention? Intervention { get; set; }
}