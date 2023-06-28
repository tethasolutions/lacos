using Lacos.GestioneCommesse.Domain.Docs;

namespace Lacos.GestioneCommesse.Domain.Registry;

public class Customer : FullAuditedEntity
{
    public string? CompanyName { get; set; }
    public string? Name { get; set; }
    public string? Surname { get; set; }
    public string? Notes { get; set; }
    public CustomerFiscalType FiscalType { get; set; }

    public ICollection<CustomerAddress> Addresses { get; set; }
    public ICollection<Job> Jobs { get; set; }

    public Customer()
    {
        Addresses = new List<CustomerAddress>();
        Jobs = new List<Job>();
    }
}