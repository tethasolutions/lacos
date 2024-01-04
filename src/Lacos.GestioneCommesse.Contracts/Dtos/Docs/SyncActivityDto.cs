using Lacos.GestioneCommesse.Contracts.Dtos.Enums;

namespace Lacos.GestioneCommesse.Contracts.Dtos.Docs
{
    public class SyncActivityDto : SyncBaseDto
    {
        public DateTimeOffset? StartDate { get; set; }
        public DateTimeOffset? ExpirationDate { get; set; }
        public int? RowNumber { get; set; }
        public string? Description { get; set; }
        public long? TypeId { get; set; }
        public long? SupplierId { get; set; }
        public long? AddressId { get; set; }
        public long? JobId { get; set; }
        public ActivityStatus? Status { get; set; }
    }
}