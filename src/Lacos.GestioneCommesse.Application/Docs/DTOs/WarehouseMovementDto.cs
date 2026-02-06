using Lacos.GestioneCommesse.Application.Shared;
using Lacos.GestioneCommesse.Domain.Docs;
using Lacos.GestioneCommesse.Domain.Registry;

namespace Lacos.GestioneCommesse.Application.Docs.DTOs;

public class WarehouseMovementDto : BaseEntityDto
{
    public long ProductId { get; set; }
    public DateTimeOffset MovementDate { get; set; }
    public WarehouseMovementType MovementType { get; set; }
    public int Quantity { get; set; }
    public string? Notes { get; set; }
}