namespace Lacos.GestioneCommesse.Application.Registry.DTOs
{
    public class ProductTypeDto
    {
        public long? Id { get; set; }
        public string? Code { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public bool IsReiDoor { get; set; }
        public bool IsSparePart { get; set; }
        public string? ColorHex { get; set; }
    }
}
