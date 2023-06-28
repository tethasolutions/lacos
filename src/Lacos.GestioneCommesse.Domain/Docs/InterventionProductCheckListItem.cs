using Lacos.GestioneCommesse.Domain.Registry;

namespace Lacos.GestioneCommesse.Domain.Docs;

public class InterventionProductCheckListItem : FullAuditedEntity
{
    public string? Description { get; set; }
    public InterventionProductCheckListItemOutcome? Outcome { get; set; }
    public string? Notes { get; set; }

    public long? OperatorId { get; set; }
    public Operator? Operator { get; set; }

    public long InterventionProductId { get; set; }
    public InterventionProduct? InterventionProduct { get; set; }
}