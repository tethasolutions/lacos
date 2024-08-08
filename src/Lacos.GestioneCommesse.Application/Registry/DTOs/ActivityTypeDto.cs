namespace Lacos.GestioneCommesse.Application.Registry.DTOs
{
    public class ActivityTypeDto
    {
        public long? Id { get; set; }
        public string? Name { get; set; }
        public bool PictureRequired { get; set; }
        public bool IsInternal { get; set; }
        public bool IsExternal { get; set; }
        public string? ColorHex { get; set; }

        public string? StatusLabel0 { get; set; }
        public string? StatusLabel1 { get; set; }
        public string? StatusLabel2 { get; set; }
        public string? StatusLabel3 { get; set; }
        public bool? InfluenceJobStatus { get; set; }
    }
}
