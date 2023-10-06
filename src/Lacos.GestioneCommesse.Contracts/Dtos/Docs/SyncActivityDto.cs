namespace Lacos.GestioneCommesse.Contracts.Dtos.Docs
{
    public class SyncActivityDto:SyncBaseDto
    {
        public int? RowNumber { get; set; }
        public string? Description { get; set; }
        public long? CustomerAddressId { get; set; }
        public long? JobId { get; set; }
        public long? TypeId { get; set; }
        public long? SourceTicketId { get; set; }
        public long? SourcePuchaseOrderId { get; set; }
    }
}
