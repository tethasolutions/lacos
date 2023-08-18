namespace Lacos.GestioneCommesse.Domain.Registry;

public class OperatorDocument : FullAuditedEntity
{
    public string? OriginalFilename { get; set; }
    public string? FileName { get; set; }

    public long OperatorId { get; set; }
    public Operator? Operator { get; set; }
}