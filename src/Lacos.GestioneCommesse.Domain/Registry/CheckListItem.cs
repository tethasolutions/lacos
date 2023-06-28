namespace Lacos.GestioneCommesse.Domain.Registry;

public class CheckListItem : FullAuditedEntity
{
    public long ProductTypeId { get; set; }
    public ProductType? ProductType { get; set; }

    public long ActivityTypeId { get; set; }
    public ActivityType? ActivityType { get; set; }

    public string? Description { get; set; }
}