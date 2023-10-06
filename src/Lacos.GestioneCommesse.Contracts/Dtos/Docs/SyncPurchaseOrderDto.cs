using Lacos.GestioneCommesse.Contracts.Dtos.Enums;

namespace Lacos.GestioneCommesse.Contracts.Dtos.Docs
{
    public class SyncPurchaseOrderDto:SyncBaseDto
    {
        public string? Description { get; set; }
        public PurchaseOrderStatus? Status { get; set; }
        public long? InterventionId { get; set; }
        public long? CustomerId { get; set; }

    }
    
}
