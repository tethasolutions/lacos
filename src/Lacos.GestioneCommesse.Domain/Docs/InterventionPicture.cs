namespace Lacos.GestioneCommesse.Domain.Docs;

public class InterventionPicture : FullAuditedEntity
{
    public string? FileName { get; set; }
    
    public long InterventionId { get; set; }
    public Intervention? Intervention { get; set; }
}