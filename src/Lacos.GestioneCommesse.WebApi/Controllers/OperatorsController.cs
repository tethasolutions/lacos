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
using Lacos.GestioneCommesse.Framework.Configuration;
using Lacos.GestioneCommesse.Framework.IO;
using Microsoft.AspNetCore.Authorization;

namespace Lacos.GestioneCommesse.WebApi.Controllers;

[RequireUser]
public class OperatorsController : LacosApiController
{
    private readonly IOperatorService operatorService;
    private readonly ILacosConfiguration configuration;
    private readonly IMimeTypeProvider mimeTypeProvider;

    public OperatorsController(IOperatorService operatorService, ILacosConfiguration configuration, IMimeTypeProvider mimeTypeProvider)
    {
        this.operatorService = operatorService;
        this.configuration = configuration;
        this.mimeTypeProvider = mimeTypeProvider;
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

    [HttpGet("{id}/all-documents")]
    public async Task<List<OperatorDocumentReadModel>> GetAllOperatorDocuments(long documentId)
    {
        var docmumentOperator = (await operatorService.GetAllOperatorDocuments(documentId)).ToList();
        return docmumentOperator;
    }

    //[HttpPost("document/{operatorId}/{description}/{fileName}")]
    //public async Task<IActionResult> CreateDocument(long operatorId, string description, string fileName)
    //{
    //    var files = HttpContext.Request.Form.Files;
    //    var provider = new MultipartMemoryStreamProvider();
    //    // var postedFile = await Request.Content.ReadAsMultipartAsync(provider);
    //    foreach (var file in provider.Contents)
    //    {
    //        // var filename = file.Headers.ContentDisposition.FileName.Trim('\"');
    //        var buffer = await file.ReadAsByteArrayAsync();
    //        // Do whatever you want with filename and its binary data.
    //        // var str = System.Text.Encoding.Default.GetString(buffer);
    //    }
    //    return Ok(2);
    //}

    //[HttpPut("document/{id}/{description}/{fileName}")]
    //public async Task<IActionResult> UpdateDocument(long id, string description, string fileName)
    //{
    //    var files = HttpContext.Request.Form.Files;
    //    var provider = new MultipartMemoryStreamProvider();
    //    // var postedFile = await Request.Content.ReadAsMultipartAsync(provider);
    //    foreach (var file in provider.Contents)
    //    {
    //        // var filename = file.Headers.ContentDisposition.FileName.Trim('\"');
    //        var buffer = await file.ReadAsByteArrayAsync();
    //        // Do whatever you want with filename and its binary data.
    //        // var str = System.Text.Encoding.Default.GetString(buffer);
    //    }
    //    return NoContent();
    //}
    
    [AllowAnonymous]
    [HttpGet("document/download-file/{fileName}")]
    public async Task<FileResult> DownloadAttachment(string fileName)
    {
        fileName = Uri.UnescapeDataString(fileName);
        OperatorDocumentReadModel operatorDocument = (await operatorService.DownloadOperatorDocument(fileName));
        
        var folder = configuration.AttachmentsPath;
        Directory.CreateDirectory(folder);
        var path = Path.Combine(folder, fileName);

        Stream stream = System.IO.File.OpenRead(path);
        return File(stream, mimeTypeProvider.Provide(fileName), operatorDocument.Description);
    }

    [HttpPost("document/upload-file")]
    public async Task<IActionResult> UploadOperatorDocument()
    {
        var file = Request.Form.Files.FirstOrDefault();
        if (file == null)
        {
            return BadRequest();
        }
        var fileName = await SaveFile(file);
        return Ok(new
        {
            fileName,
            originalFileName = Path.GetFileName(file.FileName)
        });
    }

    [HttpPost("document/remove-file")]
    public async Task<IActionResult> DeleteFile()
    {
        return Ok();
    }

    private async Task<string> SaveFile(IFormFile file)
    {
        var extension = Path.GetExtension(file.FileName);
        var fileName = Guid.NewGuid() + extension;
        var folder = configuration.AttachmentsPath;
        Directory.CreateDirectory(folder);
        var path = Path.Combine(folder, fileName);
        await using (var stream = file.OpenReadStream())
        {
            await using (var fileStream = System.IO.File.OpenWrite(path))
            {
                await stream.CopyToAsync(fileStream);
            }
        }
        return fileName;
    }

}
