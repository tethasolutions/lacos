namespace Lacos.GestioneCommesse.Contracts.Dtos.Docs
{
    public class SyncJobDto:SyncBaseDto
    {
        public int? Number { get; set; }
        public int? Year { get; set; }
        public DateTimeOffset JobDate { get; set; }
        public string? Description { get; set; }
        public long? CustomerId { get; set; }

    }
}
