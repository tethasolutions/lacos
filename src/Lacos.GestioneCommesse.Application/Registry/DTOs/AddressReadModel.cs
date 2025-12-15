namespace Lacos.GestioneCommesse.Application.Registry.DTOs
{
    public class AddressReadModel
    {
        public long? Id { get; set; }
        public long? CustomerId { get; set; }
        public string? CustomerName { get; set; }
        public string Description { get; set; }
        public string City { get; set; }
        public string StreetAddress { get; set; }
        public string Province { get; set; }
        public string? ZipCode { get; set; }
        public string? FullAddressForDistance { get; set; }
        public decimal? DistanceKm { get; set; } 
        public bool? IsInsideAreaC { get; set; }
    }
}
