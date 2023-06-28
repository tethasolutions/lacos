using Lacos.GestioneCommesse.Domain.Registry;

namespace Lacos.GestioneCommesse.Domain.Docs;

public class InterventionProductPicture : FullAuditedEntity
{
    public string? FileName { get; set; }

    public InterventionProductPictureType Type { get; set; }

    public string? Notes { get; set; }

    public long OperatorId { get; set; }
    public Operator? Operator { get; set; }

    public long InterventionProductId { get; set; }
    public InterventionProduct? InterventionProduct { get; set; }
}