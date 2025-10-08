namespace Lacos.GestioneCommesse.Application.Registry.DTOs
{
    public class MaintenancePriceListItemDto
    {
        public long? Id { get; set; }
        public long MaintenancePriceListId { get; set; }
        public string? Description { get; set; }
        public decimal ServiceCallFee { get; set; }
        public decimal TravelFee { get; set; }
        public int LimitKm { get; set; }
        public decimal ExtraFee { get; set; }
    }
}
