using Lacos.GestioneCommesse.Domain.Docs;

namespace Lacos.GestioneCommesse.Domain.Registry;

public class ContactAddress : FullAuditedEntity
{
    public string? Description { get; set; }
    public string? City { get; set; }
    public string? StreetAddress { get; set; }
    public string? Province { get; set; }
    public string? ZipCode { get; set; }
    public string? Telephone { get; set; }
    public string? Email { get; set; }
    public bool IsMainAddress { get; set; }

    public long ContactId { get; set; }
    public Contact? Contact { get; set; }

    public ICollection<Job> Jobs { get; set; }

    public ContactAddress()
    {
        Jobs = new List<Job>();
    }
}