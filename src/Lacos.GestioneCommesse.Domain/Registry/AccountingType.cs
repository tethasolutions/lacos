
using Lacos.GestioneCommesse.Domain.Docs;

namespace Lacos.GestioneCommesse.Domain.Registry;

public class AccountingType : FullAuditedEntity
{
    public string Name { get; set; }
    public string? Description { get; set; }
    public bool GenerateAlert { get; set; }
    public ICollection<JobAccounting> JobAccountings { get; set; }

    public AccountingType()
    {
        JobAccountings = new List<JobAccounting>();
    }
}