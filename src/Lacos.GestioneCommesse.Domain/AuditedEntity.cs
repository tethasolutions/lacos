namespace Lacos.GestioneCommesse.Domain;

public abstract class AuditedEntity : BaseEntity
{
    public DateTimeOffset CreatedOn { get; set; }
    public string? CreatedBy { get; set; }
    public long? CreatedById { get; set; }
    public DateTimeOffset? EditedOn { get; set; }
    public string? EditedBy { get; set; }
    public long? EditedById { get; set; }
}