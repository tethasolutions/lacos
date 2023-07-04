namespace Lacos.GestioneCommesse.Domain.Docs;

public class Note : FullAuditedEntity
{
    public string? Value { get; set; }
    
    public long? JobId { get; set; }
    public Job? Job { get; set; }

    public long? OrderId { get; set; }
    public Order? Order { get; set; }

    public long? QuotationId { get; set; }
    public Quotation? Quotation { get; set; }

    public long? ActivityId { get; set; }
    public Activity? Activity { get; set; }

    public ICollection<NoteAttachment> Attachments { get; set; }

    public Note()
    {
        Attachments = new List<NoteAttachment>();
    }
}