namespace Lacos.GestioneCommesse.Application.Docs.DTOs
{
    public class PurchaseOrderAttachmentReadModel
    {
        public long Id { get; set; }
        public string? DisplayName { get; set; }
        public string? FileName { get; set; }
        public bool IsAdminDocument { get; set; }
    }
}
