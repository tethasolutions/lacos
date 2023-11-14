using Lacos.GestioneCommesse.Contracts.Dtos.Enums;

namespace Lacos.GestioneCommesse.Contracts.Dtos.Docs
{
    public class SyncPurchaseOrderDto:SyncBaseDto
    {

        public int? Number { get; set; }
        public int? Year { get; set; }
        public DateTimeOffset? Date { get; set; }
        public string? Description { get; set; }
        public PurchaseOrderStatus? Status { get; set; }
        public long? JobId { get; set; }
        public long? SupplierId { get; set; }

    }
    
}
