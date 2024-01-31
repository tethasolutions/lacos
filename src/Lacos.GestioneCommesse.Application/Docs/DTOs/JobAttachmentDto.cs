namespace Lacos.GestioneCommesse.Application.Docs.DTOs
{
    public class JobAttachmentDto
    {
        public long? Id { get; set; }
        public string? DisplayName { get; set; }
        public string? FileName { get; set; }
        public long JobId { get; set; }
    }
}
