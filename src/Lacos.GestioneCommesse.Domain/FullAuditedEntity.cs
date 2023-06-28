namespace Lacos.GestioneCommesse.Domain;

public abstract class FullAuditedEntity : AuditedEntity, ISoftDelete
{
    public DateTimeOffset? DeletedOn { get; set; }
    public string? DeletedBy { get; set; }
    public long? DeletedById { get; set; }
    public bool IsDeleted { get; set; }
}