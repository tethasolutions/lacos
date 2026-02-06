using Lacos.GestioneCommesse.Domain.Registry;

namespace Lacos.GestioneCommesse.Domain.Docs
{
    public class WarehouseMovement : FullAuditedEntity, ILogEntity
    {
        public long ProductId { get; set; }
        public Product? Product { get; set; }
        public DateTimeOffset MovementDate { get; set; }
        public WarehouseMovementType MovementType { get; set; }
        public int Quantity { get; set; }
        public string? Notes { get; set; }
    }
    
}

