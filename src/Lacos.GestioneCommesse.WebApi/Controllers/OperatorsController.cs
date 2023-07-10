using Lacos.GestioneCommesse.WebApi.Auth;
using Microsoft.AspNetCore.Mvc;
using Lacos.GestioneCommesse.Application.Vehicles.DTOs;
using Kendo.Mvc.UI;
using Lacos.GestioneCommesse.Domain.Docs;
using Lacos.GestioneCommesse.Application.Operators.DTOs;

namespace Lacos.GestioneCommesse.WebApi.Controllers;

[RequireUser]
public class OperatorsController : LacosApiController
{
    [HttpGet("operators")]
    public async Task<DataSourceResult> GetOperators([DataSourceRequest] DataSourceRequest request)
    {
        List<OperatorReadModel> operators = new List<OperatorReadModel>
        {
            new OperatorReadModel
            {
                Id = 1,
                Email = "mario@gmail.com",
                ColorHex = "green",
                Name = "Mario Maroni",
                DefaultVehicleId = 2,
                DefaultVehicle = new VehicleDto
                {
                    Id = 2,
                    Name = "Camion",
                    Plate = "MI63414",
                    Notes = "autista Fernando"
                },
                Documents = new List<OperatorDocumentDto>
                {
                    new OperatorDocumentDto
                    {
                        Id = 1,
                        Description = "Documento 1",
                        FileName = "document1.jpg"
                    },
                    new OperatorDocumentDto
                    {
                        Id = 2,
                        Description = "Documento 2",
                        FileName = "document2.jpg"
                    }
                }
            },
            new OperatorReadModel
            {
                Id = 2,
                Email = "carlo@gmail.com",
                ColorHex = "red",
                Name = "Carlo Rossi",
                DefaultVehicleId = 3,
                DefaultVehicle = new VehicleDto
                {
                    Id = 3,
                    Name = "Furgone",
                    Plate = "BL84274",
                    Notes = "autista Carlo"
                },
                Documents = new List<OperatorDocumentDto>
                {
                    new OperatorDocumentDto
                    {
                        Id = 1,
                        Description = "Documento 1",
                        FileName = "document1.jpg"
                    },
                    new OperatorDocumentDto
                    {
                        Id = 2,
                        Description = "Documento 2",
                        FileName = "document2.jpg"
                    }
                }
            },
            new OperatorReadModel
            {
                Id = 3,
                Email = "fernando@gmail.com",
                ColorHex = "blue",
                Name = "Fernando Donati",
                DefaultVehicleId = 1,
                DefaultVehicle = new VehicleDto
                {
                    Id = 1,
                    Name = "Macchina",
                    Plate = "BG92354",
                    Notes = "autista Mario"
                },
                Documents = new List<OperatorDocumentDto>
                {
                    new OperatorDocumentDto
                    {
                        Id = 1,
                        Description = "Documento 1",
                        FileName = "document1.jpg"
                    },
                    new OperatorDocumentDto
                    {
                        Id = 2,
                        Description = "Documento 2",
                        FileName = "document2.jpg"
                    }
                }
            }
        };

        DataSourceResult result = new DataSourceResult
        {
            AggregateResults = null,
            Errors = null,
            Total = 3,
            Data = operators
        };

        return result;
    }

    [HttpGet("operator-detail/{operatorId}")]
    public async Task<OperatorReadModel> GetOperatorDetail(long operatorId)
    {
        OperatorReadModel operatorDetail = new OperatorReadModel
        {
            Id = 2,
            Email = "carlo@gmail.com",
            ColorHex = "red",
            Name = "Carlo Rossi",
            DefaultVehicleId = 3,
            DefaultVehicle = new VehicleDto
            {
                Id = 2,
                Name = "Camion",
                Plate = "MI63414",
                Notes = "autista Fernando"
            },
            Documents = new List<OperatorDocumentDto>
            {
                new OperatorDocumentDto
                {
                    Id = 1,
                    Description = "Documento 1",
                    FileName = "document1.jpg"
                },
                new OperatorDocumentDto
                {
                    Id = 2,
                    Description = "Documento 2",
                    FileName = "document2.jpg"
                }
            }
        };

        return operatorDetail;
    }

    [HttpPut("operator/{id}")]
    public async Task<IActionResult> UpdateOperator(long id, [FromBody] OperatorDto request)
    {
        return NoContent();
    }

    [HttpPost("operator")]
    public async Task<IActionResult> CreateOperator([FromBody] OperatorDto request)
    {
        return Ok(2);
    }

    [HttpDelete("operator/{id}")]
    public async Task<IActionResult> DeleteOperator(long id)
    {
        return Ok();
    }
}
