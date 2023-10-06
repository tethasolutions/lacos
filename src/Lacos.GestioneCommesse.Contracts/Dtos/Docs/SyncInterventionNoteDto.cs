namespace Lacos.GestioneCommesse.Contracts.Dtos.Docs
{
    public class SyncInterventionNoteDto:SyncBaseDto

    {
    public string? PictureFileName { get; set; }
    public string? Notes { get; set; }
    public long? OperatorId { get; set; }
    public long? InterventionId { get; set; }
    }
}
