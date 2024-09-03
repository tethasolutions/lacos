namespace Lacos.GestioneCommesse.Contracts.Dtos.Registry
{
    public class SyncActivityTypeDto:SyncBaseDto
    {
        public string? Name { get; set; }
        public bool? PictureRequired { get; set; }
        public bool? IsInternal { get; set; }
        public string? ColorHex { get; set; }
        public bool IsExternal { get; set; }

        public bool? InfluenceJobStatus { get; set; }
        public string? StatusLabel0 { get; set; }
        public string? StatusLabel1 { get; set; }
        public string? StatusLabel2 { get; set; }
        public string? StatusLabel3 { get; set; }

        public IEnumerable<long> OperatorIds { get; set; } = new List<long>();
    }
}
