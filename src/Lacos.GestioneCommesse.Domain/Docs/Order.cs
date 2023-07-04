using Lacos.GestioneCommesse.Domain.Registry;

namespace Lacos.GestioneCommesse.Domain.Docs;

public class Order : FullAuditedEntity
{
    public string? Code { get; set; }
    public string? Description { get; set; }
    public DateTimeOffset? ExpirationDate { get; set; }
    public OrderStatus Status { get; set; }
    public DateTimeOffset? StatusChangedOn { get; set; }

    public long JobId { get; set; }
    public Job? Job { get; set; }

    public long SupplierId { get; set; }
    public Contact? Supplier { get; set; }

    public ICollection<Note> Notes { get; set; }

    public Order()
    {
        Notes = new List<Note>();
    }
}