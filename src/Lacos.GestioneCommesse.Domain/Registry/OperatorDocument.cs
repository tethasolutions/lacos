namespace Lacos.GestioneCommesse.Domain.Registry;

public class OperatorDocument : FullAuditedEntity
{
    public string? Description { get; set; }
    public string? FileName { get; set; }

    public long OperatorId { get; set; }
    public Operator? Operator { get; set; }
}