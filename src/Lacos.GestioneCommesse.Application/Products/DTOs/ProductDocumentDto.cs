namespace Lacos.GestioneCommesse.Application.Products.DTOs
{
    public class ProductDocumentDto
    {
        public long? Id { get; set; }
        public long? ProductId { get; set; }
        public string? OriginalFileName { get; set; }
        public string? FileName { get; set; }
        public string? Description { get; set;}
    }
}
