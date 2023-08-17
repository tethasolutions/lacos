using Lacos.GestioneCommesse.WebApi.Auth;
using Microsoft.AspNetCore.Mvc;
using Lacos.GestioneCommesse.Application.Vehicles.DTOs;
using Kendo.Mvc.UI;
using Lacos.GestioneCommesse.Domain.Docs;
using Lacos.GestioneCommesse.Application.Vehicles.Services;
using Kendo.Mvc.Extensions;
using Lacos.GestioneCommesse.Application.Registry.Services;
using Lacos.GestioneCommesse.Application.Registry.DTOs;

namespace Lacos.GestioneCommesse.WebApi.Controllers;

[RequireUser]
public class VehiclesController : LacosApiController
{
    private readonly IVehicleService vehicleService;

    public VehiclesController(IVehicleService vehicleService)
    {
        this.vehicleService = vehicleService;
    }

    [HttpGet("vehicles")]
    public async Task<ActionResult> GetVehicles([DataSourceRequest] DataSourceRequest request)
    {
        var vehicles = await vehicleService.GetVehicles();
        return Ok(await vehicles.ToDataSourceResultAsync(request));
    }

    [HttpGet("vehicle-detail/{id}")]
    public async Task<VehicleDto> GetVehicleDetail(long id)
    {
        var vehicle = await vehicleService.GetVehicle(id);
        return vehicle;
    }

    [HttpPut("vehicle/{id}")]
    public async Task<IActionResult> UpdateVehicle(long id, [FromBody] VehicleDto vehicleDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        await vehicleService.UpdateVehicle(id, vehicleDto);
        return Ok();
    }

    [HttpPost("vehicle")]
    public async Task<IActionResult> CreateVehicle([FromBody] VehicleDto vehicleDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        await vehicleService.CreateVehicle(vehicleDto);
        return Ok(vehicleDto);
    }

    [HttpDelete("vehicle/{id}")]
    public async Task<IActionResult> DeleteVehicle(long id)
    {
        return Ok();
    }

    [HttpGet("vehicles-list")]
    public async Task<List<VehicleDto>> GetVehiclesList([DataSourceRequest] DataSourceRequest request)
    {
        List<VehicleDto> vehicles = new List<VehicleDto>
        {
            new VehicleDto
            {
                Id = 1,
                Name = "Macchina",
                Plate = "BG92354",
                Notes = "autista Mario"
            },
            new VehicleDto
            {
                Id = 2,
                Name = "Camion",
                Plate = "MI63414",
                Notes = "autista Fernando"
            },
            new VehicleDto
            {
                Id = 3,
                Name = "Furgone",
                Plate = "BL84274",
                Notes = "autista Carlo"
            }
        };

        return vehicles;
    }
}