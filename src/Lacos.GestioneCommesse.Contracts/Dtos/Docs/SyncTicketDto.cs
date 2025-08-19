using Lacos.GestioneCommesse.Contracts.Dtos.Enums;

namespace Lacos.GestioneCommesse.Contracts.Dtos.Docs
{
    public class SyncTicketDto: SyncBaseDto
    {
        public int? Number { get; set; }
        public int? Year { get; set; }
        public DateTimeOffset? TicketDate { get; set; }
        public string? Description { get; set; }
        public TicketStatus? Status { get; set; }
        public long? JobId { get; set; }
        public bool? IsNew { get; set; }
        public long? CustomerId { get; set; }
        public long? ActivityId { get; set; }
        public long? OperatorId { get; set;}
        public string? ReportFileName { get; set; }
        public DateTimeOffset? ReportGeneratedOn { get; set; }
        public string? CustomerSignatureName { get; set; }
        public string? CustomerSignatureFileName { get; set; }
        
        public long? PurchaseOrderId { get; set; }
        
        public long? AddressId { get; set; }
    }

   
}
