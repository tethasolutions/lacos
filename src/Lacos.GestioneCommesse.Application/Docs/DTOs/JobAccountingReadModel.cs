using Lacos.GestioneCommesse.Application.Registry.DTOs;
using Lacos.GestioneCommesse.Application.Shared;
using Lacos.GestioneCommesse.Domain.Docs;
using Lacos.GestioneCommesse.Domain.Registry;

namespace Lacos.GestioneCommesse.Application.Docs.DTOs;

public class JobAccountingReadModel
{
    public long Id { get; set; }
    public long JobId { get; set; }
    public string? JobCode { get; set; }
    public string? JobReference { get; set; }
    public long AccountingTypeId { get; set; }
    public string? AccountingTypeName { get; set; }
    public bool? GenerateAlert { get; set; }
    public decimal Amount { get; set; }
    public string? Note { get; set; }
    public bool IsPaid { get; set; }
}