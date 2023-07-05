namespace Lacos.GestioneCommesse.Domain.Docs;

public class NoteAttachment : FullAuditedEntity
{
    public string? DisplayName { get; set; }
    public string? FileName { get; set; }

    public long NoteId { get; set; }
    public Note? Note { get; set; }
}