namespace Lacos.GestioneCommesse.Application.Docs.DTOs
{
    public class TicketAttachmentDto
    {
        public long? Id { get; set; }
        public string? Description { get; set; }
        public string? FileName { get; set; }
        public long TicketId { get; set; }
    }
}
