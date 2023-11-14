namespace Lacos.GestioneCommesse.Application.Products.DTOs
{
    public class ProductDto
    {
        public long? Id { get; set; }
        
        public string? Code { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? PictureFileName { get; set; }
        public string? QrCodePrefix { get; set; }
        public string? QrCodeNumber { get; set; }
        public long? CustomerId { get; set; }
        public long? AddressId { get; set; }
        public long? ProductTypeId { get; set; }

        public string? Location { get; set; }
        public string? SerialNumber { get; set; }
        public string? ReiType { get; set; }
        public string? ConstructorName { get; set; }
        public bool? HasPushBar { get; set; }
        public int? Year { get; set; }
        public string? VocType { get; set; }
        public int? NumberOfDoors { get; set; }

        public ICollection<ProductDocumentDto>? Documents { get; set; }
        public ProductDto()
        {
            Documents = new List<ProductDocumentDto>();
        }
    }
}
