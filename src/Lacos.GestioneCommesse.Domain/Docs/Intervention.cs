using Lacos.GestioneCommesse.Domain.Registry;

namespace Lacos.GestioneCommesse.Domain.Docs;

public class Intervention : FullAuditedEntity
{
    public DateTimeOffset Start { get; set; }
    public DateTimeOffset End { get; set; }

    public InterventionStatus Status { get; set; }

    public string? Description { get; set; }
    public string? FinalNotes { get; set; }

    public string? ReportFileName { get; set; }
    public DateTimeOffset? ReportGeneratedOn { get; set; }
    public string? CustomerSignatureName { get; set; }
    public string? CustomerSignatureFileName { get; set; }

    public long? VehicleId { get; set; }
    public Vehicle? Vehicle { get; set; }

    public long ActivityId { get; set; }
    public Activity? Activity { get; set; }

    public ICollection<Operator> Operators { get; set; }
    public ICollection<InterventionNote> Notes { get; set; }
    public ICollection<InterventionProduct> Products { get; set; }
    public ICollection<InterventionDispute> Disputes { get; set; }

    public Intervention()
    {
        Notes = new List<InterventionNote>();
        Operators = new List<Operator>();
        Products = new List<InterventionProduct>();
        Disputes = new List<InterventionDispute>();
    }

    public bool IsCompleted()
    {
        return Status is
            InterventionStatus.CompletedSuccesfully or
            InterventionStatus.CompletedUnsuccesfully;
    }
}