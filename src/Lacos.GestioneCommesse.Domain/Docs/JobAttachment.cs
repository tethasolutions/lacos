using Lacos.GestioneCommesse.Domain.Registry;

namespace Lacos.GestioneCommesse.Domain.Docs;

public class JobAttachment : FullAuditedEntity
{
    public string? DisplayName { get; set; }
    public string? FileName { get; set; }

    public long JobId { get; set; }
    public Job? Job { get; set; }
}