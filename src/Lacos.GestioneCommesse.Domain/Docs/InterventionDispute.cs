using Lacos.GestioneCommesse.Domain.Security;

namespace Lacos.GestioneCommesse.Domain.Docs;

public class InterventionDispute : FullAuditedEntity
{
    public long? OperatorId { get; set; }
    public User? Operator { get; set; }

    public string? Notes { get; set; }
    public InterventionDisputeStatus Status { get; set; }

    public long InterventionId { get; set; }
    public Intervention? Intervention { get; set; }
}