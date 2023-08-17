using Lacos.GestioneCommesse.WebApi.Auth;
using Microsoft.AspNetCore.Mvc;
using Lacos.GestioneCommesse.Application.Vehicles.DTOs;
using Kendo.Mvc.UI;
using Lacos.GestioneCommesse.Domain.Docs;
using Lacos.GestioneCommesse.Application.Operators.DTOs;
using System.Net.Http;
using System.Web.Http;
using Kendo.Mvc.Extensions;
using Lacos.GestioneCommesse.Application.Operators.Services;
using Lacos.GestioneCommesse.Application.Customers.Services;

namespace Lacos.GestioneCommesse.WebApi.Controllers;

[RequireUser]
public class OperatorsController : LacosApiController
{
    private readonly IOperatorService operatorService;
    public OperatorsController(IOperatorService operatorService)
    {
        this.operatorService = operatorService;
    }

    [HttpGet("operators")]
    public async Task<DataSourceResult> GetOperators([DataSourceRequest] DataSourceRequest request)
    {
        var operators = operatorService.GetOperators();
        return await operators.ToDataSourceResultAsync(request);
    }

    [HttpGet("operator-detail/{operatorId}")]
    public async Task<OperatorReadModel> GetOperatorDetail(long operatorId)
    {
        var singleOperator = await operatorService.GetOperator(operatorId);
        return singleOperator;
    }

    [HttpPut("operator/{id}")]
    public async Task<IActionResult> UpdateOperator(long id, [FromBody] OperatorDto operatorDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        await operatorService.UpdateOperator(id, operatorDto);
        return Ok();
    }

    [HttpPost("operator")]
    public async Task<IActionResult> CreateOperator([FromBody] OperatorDto operatorDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        await operatorService.CreateOperator(operatorDto);
        return Ok(operatorDto);
    }

    [HttpDelete("operator/{id}")]
    public async Task<IActionResult> DeleteOperator(long id)
    {
        await operatorService.DeleteOperator(id);
        return Ok();
    }

    [HttpGet("operator-document/{documentId}")]
    public async Task<OperatorDocumentDto> GetOperatorDocument(long documentId)
    {
        var docmumentOperator = await operatorService.GetOperatorDocmument(documentId);
        return docmumentOperator;
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
