using Lacos.GestioneCommesse.Contracts.Dtos.Enums;

namespace Lacos.GestioneCommesse.Contracts.Dtos.Docs
{
    public class SyncInterventionProductPictureDto:SyncBaseDto
    {
        public string? FileName { get; set; }
        public InterventionProductPictureType? Type { get; set; }
        public string? Notes { get; set; }
        public long? OperatorId { get; set; }
        public long? InterventionProductId { get; set; }
    }

  
}
