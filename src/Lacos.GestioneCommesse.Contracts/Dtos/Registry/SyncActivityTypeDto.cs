namespace Lacos.GestioneCommesse.Contracts.Dtos.Registry
{
    public class SyncActivityTypeDto:SyncBaseDto
    {
        public string? Name { get; set; }
        public bool? PictureRequired { get; set; }

        public bool? IsInternal { get; set; }
        public string? ColorHex { get; set; }

        public IEnumerable<long> OperatorIds { get; set; } = new List<long>();

    }
}
