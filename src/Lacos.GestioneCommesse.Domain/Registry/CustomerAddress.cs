using Lacos.GestioneCommesse.Domain.Docs;

namespace Lacos.GestioneCommesse.Domain.Registry;

public class CustomerAddress : FullAuditedEntity
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

    public long CustomerId { get; set; }
    public Customer? Customer { get; set; }

    public ICollection<Activity> Activities { get; set; }
    public ICollection<Product> Products { get; set; }
    public ICollection<Ticket> Tickets { get; set; }

    public CustomerAddress()
    {
        Activities = new List<Activity>();
        Products = new List<Product>();
        Tickets = new List<Ticket>();
    }
}