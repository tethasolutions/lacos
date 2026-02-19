using Lacos.GestioneCommesse.Application.Shared;
using Lacos.GestioneCommesse.Domain.Docs;
using Lacos.GestioneCommesse.Domain.Registry;

namespace Lacos.GestioneCommesse.Application.Docs.DTOs;

public class PurchaseOrderExpenseDto : BaseEntityDto
{
    public long PurchaseOrderId { get; set; }
    public long JobId { get; set; }
    public string? JobCode { get; set; }
    public string? Note { get; set; }
    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalAmount { get; set; }

}