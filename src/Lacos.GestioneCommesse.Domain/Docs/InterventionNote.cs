using Lacos.GestioneCommesse.Domain.Registry;

namespace Lacos.GestioneCommesse.Domain.Docs;

public class InterventionNote : FullAuditedEntity, ILogEntity
{
    public string? PictureFileName { get; set; }

    public string? Notes { get; set; }

    public long OperatorId { get; set; }
    public Operator? Operator { get; set; }

    public long InterventionId { get; set; }
    public Intervention? Intervention { get; set; }
}