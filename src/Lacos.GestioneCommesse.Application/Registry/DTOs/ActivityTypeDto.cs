namespace Lacos.GestioneCommesse.Application.Registry.DTOs
{
    public class ActivityTypeDto
    {
        public long? Id { get; set; }
        public string? Name { get; set; }
        public bool PictureRequired { get; set; }
        public bool IsInternal { get; set; }
        public string? ColorHex { get; set; }
    }
}
