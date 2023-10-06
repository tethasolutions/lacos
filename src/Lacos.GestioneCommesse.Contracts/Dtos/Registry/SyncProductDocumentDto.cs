namespace Lacos.GestioneCommesse.Contracts.Dtos.Registry
{
    public class SyncProductDocumentDto:SyncBaseDto
    {
        public string? OriginalFilename { get; set; }
        public string? FileName { get; set; }
        public string? Description { get; set; }
        public long? ProductId { get; set; }
    }
}
