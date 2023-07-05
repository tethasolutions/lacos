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
        List<OperatorDto> operators = new List<OperatorDto>
        {
            new OperatorDto
            {
                Id = 1,
                Email = "mario@gmail.com",
                ColorHex = "green",
                Name = "Mario Maroni",
                DefaultVehicleId = 2,
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
            new OperatorDto
            {
                Id = 2,
                Email = "carlo@gmail.com",
                ColorHex = "red",
                Name = "Carlo Rossi",
                DefaultVehicleId = 3,
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
            new OperatorDto
            {
                Id = 3,
                Email = "fernando@gmail.com",
                ColorHex = "blue",
                Name = "Fernando Donati",
                DefaultVehicleId = 1,
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
    public async Task<OperatorDto> GetOperatorDetail(long operatorId)
    {
        OperatorDto operatorDetail = new OperatorDto
        {
            Id = 2,
            Email = "carlo@gmail.com",
            ColorHex = "red",
            Name = "Carlo Rossi",
            DefaultVehicleId = 3,
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
