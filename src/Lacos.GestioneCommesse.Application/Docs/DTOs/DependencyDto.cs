using Lacos.GestioneCommesse.Domain.Docs;

namespace Lacos.GestioneCommesse.Application.Docs.DTOs;

public class DependencyDto
{
    public IEnumerable<long> ActivityDependenciesId { get; set; }

    public IEnumerable<long> PurchaseOrderDependenciesId { get; set; }

}