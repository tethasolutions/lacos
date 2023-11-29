using Lacos.GestioneCommesse.Domain.Registry;

namespace Lacos.GestioneCommesse.Domain.Docs;

public class ActivityAttachment : FullAuditedEntity
{
    public string? DisplayName { get; set; }
    public string? FileName { get; set; }

    public long ActivityId { get; set; }
    public Activity? Activity { get; set; }
}