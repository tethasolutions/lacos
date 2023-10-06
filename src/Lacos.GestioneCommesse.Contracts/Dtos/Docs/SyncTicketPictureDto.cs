namespace Lacos.GestioneCommesse.Contracts.Dtos.Docs
{
    public class SyncTicketPictureDto:SyncBaseDto
    {
        public string? FileName { get; set; }
        public string? Description { get; set; }
        public long? TicketId { get; set; }
    }
}
