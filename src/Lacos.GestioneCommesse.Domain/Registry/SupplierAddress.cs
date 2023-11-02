using Lacos.GestioneCommesse.Domain.Docs;

namespace Lacos.GestioneCommesse.Domain.Registry;

public class SupplierAddress : FullAuditedEntity
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

    public long SupplierId { get; set; }
    public Supplier? Supplier { get; set; }

}