

using Lacos.GestioneCommesse.Contracts.Dtos.Enums;

namespace Lacos.GestioneCommesse.Contracts.Dtos.Docs
{
    public class SyncInterventionDto:SyncBaseDto
    {
        public DateTimeOffset? Start { get; set; }
        public DateTimeOffset? End { get; set; }
        public InterventionStatus? Status { get; set; }
        public bool? ToBeReschedule { get; set; }
        public string? Description { get; set; }
        public string? FinalNotes { get; set; }
        public string? RescheduleNotes { get; set; }
        public string? ReportFileName { get; set; }
        public DateTimeOffset? ReportGeneratedOn { get; set; }
        public string? CustomerSignatureName { get; set; }
        public string? CustomerSignatureFileName { get; set; }
        public long? VehicleId { get; set; }
        public long? ActivityId { get; set; }
        public IEnumerable<long> OperatorIds { get; set; } = new List<long>();

        public decimal ServiceCallFee { get; set; }
        public decimal ServiceFee { get; set; }
        public decimal TravelFee { get; set; }
        public decimal ExtraFee { get; set; }
    }
    
}
