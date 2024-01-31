namespace Lacos.GestioneCommesse.Application.Docs.DTOs
{
    public class PurchaseOrderAttachmentDto
    {
        public long? Id { get; set; }
        public string? DisplayName { get; set; }
        public string? FileName { get; set; }
        public long PurchaseOrderId { get; set; }
    }
}
