using Lacos.GestioneCommesse.Application.Docs.DTOs;

namespace Lacos.GestioneCommesse.Application.Docs.Services;

public interface IWarehouseMovementService
{
    IQueryable<WarehouseMovementReadModel> GetWarehouseMovements(long productId);

    Task<WarehouseMovementDto> GetWarehouseMovement(long id);

    Task<WarehouseMovementDto> CreateWarehouseMovement(WarehouseMovementDto dto);

    Task<WarehouseMovementDto> UpdateWarehouseMovement(long id, WarehouseMovementDto dto);

    Task DeleteWarehouseMovement(long id);
}
