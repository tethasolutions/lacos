namespace Lacos.GestioneCommesse.Application.Registry.DTOs
{
    public class MaintenancePriceListDto
    {
        public long? Id { get; set; }
        public string? Description { get; set; }
        public long HourlyRate { get; set; }
        public ICollection<MaintenancePriceListItemDto> Items { get; set; }
    }
}
