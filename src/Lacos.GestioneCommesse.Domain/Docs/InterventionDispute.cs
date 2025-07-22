namespace Lacos.GestioneCommesse.Domain.Docs;

public class InterventionDispute : FullAuditedEntity, ILogEntity
{
    public string? Description { get; set; }

    public long InterventionId { get; set; }
    public Intervention? Intervention { get; set; }
}