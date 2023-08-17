using Lacos.GestioneCommesse.WebApi.Auth;
using Microsoft.AspNetCore.Mvc;
using Lacos.GestioneCommesse.Application.Vehicles.DTOs;
using Kendo.Mvc.UI;
using Lacos.GestioneCommesse.Domain.Docs;
using Lacos.GestioneCommesse.Application.Operators.DTOs;
using System.Net.Http;
using System.Web.Http;
using Lacos.GestioneCommesse.Application.Operators.Services;
using Lacos.GestioneCommesse.Application.Vehicles.Services;
using Kendo.Mvc.Extensions;

namespace Lacos.GestioneCommesse.WebApi.Controllers;

[RequireUser]
public class OperatorsController : LacosApiController
{
    private readonly IOperatorService operatorService;

    [HttpGet("operators")]
    public async Task<ActionResult> GetOperators([DataSourceRequest] DataSourceRequest request)
    {
        var operators = await operatorService.GetOperators();
        return Ok(await operators.ToDataSourceResultAsync(request));
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
                    OperatorId = 2,
                    Description = "Documento 1",
                    FileName = "document1.jpg"
                },
                new OperatorDocumentDto
                {
                    Id = 2,
                    OperatorId = 2,
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

    [HttpGet("operator-document/{documentId}")]
    public async Task<OperatorDocumentDto> GetOperatorDocument(long documentId)
    {
        var document = new OperatorDocumentDto
        {
            Id = 2,
            OperatorId = 2,
            Description = "Documento 2",
            FileName = "document2.jpg"
        };
        return document;
    }

    [HttpPost("document/{operatorId}/{description}/{fileName}")]
    public async Task<IActionResult> CreateDocument(long operatorId, string description, string fileName)
    {
        var files = HttpContext.Request.Form.Files;
        var provider = new MultipartMemoryStreamProvider();
        // var postedFile = await Request.Content.ReadAsMultipartAsync(provider);
        foreach (var file in provider.Contents)
        {
            // var filename = file.Headers.ContentDisposition.FileName.Trim('\"');
            var buffer = await file.ReadAsByteArrayAsync();
            // Do whatever you want with filename and its binary data.
            // var str = System.Text.Encoding.Default.GetString(buffer);
        }
        return Ok(2);
    }

    [HttpPut("document/{id}/{description}/{fileName}")]
    public async Task<IActionResult> UpdateDocument(long id, string description, string fileName)
    {
        var files = HttpContext.Request.Form.Files;
        var provider = new MultipartMemoryStreamProvider();
        // var postedFile = await Request.Content.ReadAsMultipartAsync(provider);
        foreach (var file in provider.Contents)
        {
            // var filename = file.Headers.ContentDisposition.FileName.Trim('\"');
            var buffer = await file.ReadAsByteArrayAsync();
            // Do whatever you want with filename and its binary data.
            // var str = System.Text.Encoding.Default.GetString(buffer);
        }
        return NoContent();
    }

    [HttpDelete("document/{id}")]
    public async Task<IActionResult> DeleteDocument(long id)
    {
        return Ok();
    }
}
