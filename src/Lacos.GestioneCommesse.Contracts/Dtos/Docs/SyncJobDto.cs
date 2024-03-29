using Lacos.GestioneCommesse.Contracts.Dtos.Enums;

namespace Lacos.GestioneCommesse.Contracts.Dtos.Docs
{

    public class SyncJobDto:SyncBaseDto
    {

        public int? Number { get; set; }
        public int? Year { get; set; }
        public DateTimeOffset? JobDate { get; set; }
        public DateTimeOffset? ExpirationDate { get; set; }
        public string? Description { get; set; }
        public string? Reference { get; set; }
        public bool HasHighPriority { get; set; }
        public long? CustomerId { get; set; }
        public long? AddressId { get; set; }
        public bool? IsInternalJob { get; set; }
        public JobStatus? Status { get; set; }
        public long? ReferentId { get; set; }

    }

}
