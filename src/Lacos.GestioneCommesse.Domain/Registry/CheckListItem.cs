namespace Lacos.GestioneCommesse.Domain.Registry;

public class CheckListItem : FullAuditedEntity
{
    public string? Description { get; set; }

    public long CheckListId { get; set; }
    public CheckList? CheckList { get; set; }
}