using Lacos.GestioneCommesse.Application.Registry.DTOs;
using Lacos.GestioneCommesse.Application.Shared;
using Lacos.GestioneCommesse.Domain.Docs;
using Lacos.GestioneCommesse.Domain.Registry;

namespace Lacos.GestioneCommesse.Application.Docs.DTOs;

public class JobAccountingDto : BaseEntityDto
{
    public long JobId { get; set; }
    public long AccountingTypeId { get; set; }
    public decimal Amount { get; set; }
    public string? Note { get; set; }
    public bool IsPaid { get; set; }
    public ICollection<long>? TargetOperators { get; set; }
}