using Lacos.GestioneCommesse.WebApi.Auth;
using Microsoft.AspNetCore.Mvc;
using Lacos.GestioneCommesse.Application.Vehicles.DTOs;
using Kendo.Mvc.UI;
using Lacos.GestioneCommesse.Domain.Docs;

namespace Lacos.GestioneCommesse.WebApi.Controllers;

[RequireUser]
public class VehiclesController : LacosApiController
{
    public VehiclesController()
    {
    }

    [HttpGet("vehicles")]
    public async Task<DataSourceResult> GetVehicles([DataSourceRequest] DataSourceRequest request)
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

        DataSourceResult result = new DataSourceResult
        {
            AggregateResults = null,
            Errors = null,
            Total = 3,
            Data = vehicles
        };

        return result;
    }

    [HttpGet("vehicle-detail/{vehicleId}")]
    public async Task<VehicleDto> GetVehicleDetail(long vehicleId)
    {
        VehicleDto vehicleDetail = new VehicleDto
        {
            Id = 2,
            Name = "Camion",
            Plate = "MI63414",
            Notes = "autista Fernando"
        };

        return vehicleDetail;
    }

    [HttpPut("vehicle/{id}")]
    public async Task<IActionResult> UpdateVehicle(long id, [FromBody] VehicleDto request)
    {
        return NoContent();
    }

    [HttpPost("vehicle")]
    public async Task<IActionResult> CreateVehicle([FromBody] VehicleDto request)
    {
        return Ok(2);
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