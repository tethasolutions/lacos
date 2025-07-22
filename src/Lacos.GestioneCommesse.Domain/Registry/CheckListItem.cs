namespace Lacos.GestioneCommesse.Domain.Registry;

public class CheckListItem : FullAuditedEntity, ILogEntity
{
    public string? Description { get; set; }

    public long CheckListId { get; set; }
    public CheckList? CheckList { get; set; }
}