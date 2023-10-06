namespace Lacos.GestioneCommesse.Contracts.Dtos.Registry
{
    public class SyncOperatorDocumentDto:SyncBaseDto
    {
        public string? OriginalFilename { get; set; }
        public string? FileName { get; set; }

        public long? OperatorId { get; set; }
    }
}
