using Lacos.GestioneCommesse.Application.Shared;
using Lacos.GestioneCommesse.Domain.Docs;

namespace Lacos.GestioneCommesse.Application.Docs.DTOs;

public class WarehouseMovementReadModel : BaseEntityDto
{
    public long ProductId { get; set; }
    public string? ProductCode { get; set; }
    public string? ProductName { get; set; }
    public DateTimeOffset MovementDate { get; set; }
    public WarehouseMovementType MovementType { get; set; }
    public int Quantity { get; set; }
    public string? Notes { get; set; }
}
