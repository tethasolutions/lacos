namespace Lacos.GestioneCommesse.Domain.Registry;

public class CheckList : FullAuditedEntity
{
    public string? PictureFileName { get; set; }
    public string? Description { get; set; }

    public long ProductTypeId { get; set; }
    public ProductType? ProductType { get; set; }

    public long ActivityTypeId { get; set; }
    public ActivityType? ActivityType { get; set; }

    public ICollection<CheckListItem> Items { get; set; }

    public CheckList()
    {
        Items = new List<CheckListItem>();
    }
}