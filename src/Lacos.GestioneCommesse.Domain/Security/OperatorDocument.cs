namespace Lacos.GestioneCommesse.Domain.Security;

public class OperatorDocument : FullAuditedEntity
{
    public string? Description { get; set; }
    public string? FileName { get; set; }

    public long OperatorId { get; set; }
    public User? Operator { get; set; }
}