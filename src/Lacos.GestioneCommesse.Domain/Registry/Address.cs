using Lacos.GestioneCommesse.Domain.Docs;

namespace Lacos.GestioneCommesse.Domain.Registry;

public class Address : FullAuditedEntity, ILogEntity
{
    public string? Description { get; set; }
    public string? City { get; set; }
    public string? StreetAddress { get; set; }
    public string? Province { get; set; }
    public string? ZipCode { get; set; }
    public string? Telephone { get; set; }
    public string? Email { get; set; }
    public bool IsMainAddress { get; set; }
    public string? Notes { get; set; }

    public string? JobReference { get; set; }
    public string? ContactName { get; set; }
    public string? ContactReference { get; set; }

    public long? CustomerId { get; set; }
    public Customer? Customer { get; set; }

    public long? SupplierId { get; set; }
    public Supplier? Supplier { get; set; }

    public ICollection<Job> Jobs { get; set; }
    public ICollection<Activity> Activities { get; set; }
}