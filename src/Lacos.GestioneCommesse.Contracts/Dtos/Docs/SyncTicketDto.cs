using Lacos.GestioneCommesse.Contracts.Dtos.Enums;

namespace Lacos.GestioneCommesse.Contracts.Dtos.Docs
{
    public class SyncTicketDto: SyncBaseDto
    {
        public string? Description { get; set; }
        public TicketStatus? Status { get; set; }
        public long? InterventionId { get; set; }
        public long? CustomerId { get; set; }

        public int? Number { get; set; }
        public int? Year { get; set; }
        public DateTimeOffset? TicketDate { get; set; }

    }

   
}
