namespace Lacos.GestioneCommesse.Contracts.Dtos.Docs
{
    public class SyncActivityProductDto:SyncBaseDto
    {
        public long? ProductId { get; set; }
        public long? ActivityId { get; set; }
        public string? Description { get; set; }
        public string? Location { get; set; }
    }
}
