using Lacos.GestioneCommesse.Domain.Docs;

namespace Lacos.GestioneCommesse.Application.Docs.DTOs
{
    public class JobAttachmentReadModel
    {
        public long Id { get; set; }
        public string? DisplayName { get; set; }
        public string? FileName { get; set; }
        public JobAttachmentType Type { get; set; }
    }
}
