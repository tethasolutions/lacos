using Lacos.GestioneCommesse.Domain.Docs;

namespace Lacos.GestioneCommesse.Domain.Registry;

public class Contact : FullAuditedEntity
{
    public ContactType Type { get; set; }
    public string? CompanyName { get; set; }
    public string? Name { get; set; }
    public string? Surname { get; set; }
    public ContactFiscalType FiscalType { get; set; }
    public string? ErpCode { get; set; }
    public bool Alert { get; set; }

    public ICollection<ContactAddress> Addresses { get; set; }
    public ICollection<Job> Jobs { get; set; }
    public ICollection<Order> Orders { get; set; }

    public Contact()
    {
        Addresses = new List<ContactAddress>();
        Jobs = new List<Job>();
        Orders = new List<Order>();
    }
}