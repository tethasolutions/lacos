namespace Lacos.GestioneCommesse.Contracts.Dtos.Docs
{
    public class SyncInterventionProductCheckListDto:SyncBaseDto
    {
        public string? Description { get; set; }
        public string? Notes { get; set; }
        public long? InterventionProductId { get; set; }
    }
}
