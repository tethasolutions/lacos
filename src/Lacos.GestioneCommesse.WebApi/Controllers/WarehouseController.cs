using Lacos.GestioneCommesse.Application.Docs.DTOs;
using Lacos.GestioneCommesse.Application.Docs.Services;
using Lacos.GestioneCommesse.WebApi.Auth;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Mvc;

namespace Lacos.GestioneCommesse.WebApi.Controllers;

[RequireUser]
public class WarehouseController : LacosApiController
{
    private readonly IWarehouseMovementService warehouseMovementService;

    public WarehouseController(IWarehouseMovementService warehouseMovementService)
    {
        this.warehouseMovementService = warehouseMovementService;
    }

    [HttpGet("movements/{productId}")]
    public async Task<DataSourceResult> GetWarehouseMovements(long productId, [DataSourceRequest] DataSourceRequest request)
    {
        var movements = warehouseMovementService.GetWarehouseMovements(productId);
        return await movements.ToDataSourceResultAsync(request);
    }

    [HttpGet("movement/{id}")]
    public async Task<ActionResult<WarehouseMovementDto>> GetWarehouseMovement(long id)
    {
        var movement = await warehouseMovementService.GetWarehouseMovement(id);
        return Ok(movement);
    }

    [HttpPost("movement")]
    public async Task<IActionResult> CreateWarehouseMovement([FromBody] WarehouseMovementDto request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var movement = await warehouseMovementService.CreateWarehouseMovement(request);
        return Ok(movement);
    }

    [HttpPut("movement/{id}")]
    public async Task<IActionResult> UpdateWarehouseMovement(long id, [FromBody] WarehouseMovementDto request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        await warehouseMovementService.UpdateWarehouseMovement(id, request);
        return Ok();
    }

    [HttpDelete("movement/{id}")]
    public async Task<IActionResult> DeleteWarehouseMovement(long id)
    {
        await warehouseMovementService.DeleteWarehouseMovement(id);
        return Ok();
    }
}
