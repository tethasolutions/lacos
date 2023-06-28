using Lacos.GestioneCommesse.Domain.Docs;
using Lacos.GestioneCommesse.Domain.Security;

namespace Lacos.GestioneCommesse.Domain.Registry;

public class Customer : FullAuditedEntity
{
    public string? Name { get; set; }
    public string? Notes { get; set; }
    public CustomerFiscalType FiscalType { get; set; }
    public bool CanGenerateTickets { get; set; }

    public long? UserId { get; set; }
    public User? User { get; set; }

    public ICollection<CustomerAddress> Addresses { get; set; }
    public ICollection<Job> Jobs { get; set; }
    public ICollection<Product> Products { get; set; }
    public ICollection<Ticket> Tickets { get; set; }
    public ICollection<PurchaseOrder> PurchaseOrders { get; set; }

    public Customer()
    {
        Addresses = new List<CustomerAddress>();
        Jobs = new List<Job>();
        Products = new List<Product>();
        Tickets = new List<Ticket>();
        PurchaseOrders = new List<PurchaseOrder>();
    }
}