using Lacos.GestioneCommesse.Application.Shared;
using Lacos.GestioneCommesse.Domain.Docs;
using Lacos.GestioneCommesse.Domain.Registry;

namespace Lacos.GestioneCommesse.Application.Docs.DTOs;

public class PurchaseOrderItemReadModel : BaseEntityDto
{
    public long ProductId { get; set; }
    public string? ProductName { get; set; }

    public decimal Quantity { get; set; }

    public long PurchaseOrderId { get; set; }
}