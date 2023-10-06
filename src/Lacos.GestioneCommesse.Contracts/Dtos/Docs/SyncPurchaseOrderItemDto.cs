namespace Lacos.GestioneCommesse.Contracts.Dtos.Docs
{
    public class SyncPurchaseOrderItemDto:SyncBaseDto
    {
        public long? ProductId { get; set; }
        public decimal? Quantity { get; set; }
        public long? PurchaseOrderId { get; set; }
    }
}
