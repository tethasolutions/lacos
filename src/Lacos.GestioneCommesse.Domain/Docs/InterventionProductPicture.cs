using Lacos.GestioneCommesse.Domain.Security;

namespace Lacos.GestioneCommesse.Domain.Docs;

public class InterventionProductPicture : FullAuditedEntity
{
    public string? FileName { get; set; }

    public InterventionProductPictureType Type { get; set; }

    public string? Notes { get; set; }

    public long? OperatorId { get; set; }
    public User? Operator { get; set; }

    public long InterventionProductId { get; set; }
    public InterventionProduct? InterventionProduct { get; set; }
}