using Lacos.GestioneCommesse.Domain.Registry;

namespace Lacos.GestioneCommesse.Domain.Docs;

public class JobAccounting : FullAuditedEntity
{
    public long JobId { get; set; }
    public Job? Job { get; set; }

    public long AccountingTypeId { get; set; }
    public AccountingType AccountingType { get; set; }
    public decimal Amount { get; set; }
    public string? Note { get; set; }
    public bool IsPaid { get; set; }

}
