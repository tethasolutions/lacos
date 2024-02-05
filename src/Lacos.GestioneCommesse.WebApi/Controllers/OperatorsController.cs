using Lacos.GestioneCommesse.WebApi.Auth;
using Microsoft.AspNetCore.Mvc;
using Kendo.Mvc.UI;
using Lacos.GestioneCommesse.Application.Operators.DTOs;
using Kendo.Mvc.Extensions;
using Lacos.GestioneCommesse.Application.Operators.Services;
using Lacos.GestioneCommesse.Framework.Configuration;
using Lacos.GestioneCommesse.Framework.IO;
using Lacos.GestioneCommesse.WebApi.ModelBinders;
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
    public Task<DataSourceResult> GetOperators([LacosDataSourceRequest] DataSourceRequest request)
    {
        return operatorService.GetOperators()
            .ToDataSourceResultAsync(request);
    }

    [HttpGet("operator-detail/{operatorId}")]
    public Task<OperatorDto> GetOperatorDetail(long operatorId)
    {
        return operatorService.GetOperator(operatorId);
    }

    [HttpGet("operator-byuserid/{userId}")]
    public Task<OperatorDto> GetOperatorByUserId(long userId)
    {
        return operatorService.GetOperatorByUserId(userId);
    }

    [HttpPut("operator/{id}")]
    public Task UpdateOperator(long id, OperatorDto operatorDto)
    {
        operatorDto.Id = id;

        return operatorService.UpdateOperator(operatorDto);
    }

    [HttpPost("operator")]
    public Task<OperatorDto> CreateOperator(OperatorDto operatorDto)
    {
        return operatorService.CreateOperator(operatorDto);
    }

    [HttpDelete("operator/{id}")]
    public Task DeleteOperator(long id)
    {
        return operatorService.DeleteOperator(id);
    }

    [HttpGet("{id}/all-documents")]
    public Task<IEnumerable<OperatorDocumentReadModel>> GetAllOperatorDocuments(long documentId)
    {
        return operatorService.GetAllOperatorDocuments(documentId);
    }
    
    [AllowAnonymous]
    [HttpGet("document/download-file/{fileName}/{orginalFileName}")]
    public async Task<FileResult> DownloadAttachment(string fileName,string originalFileName)
    {
       
        OperatorDocumentReadModel operatorDocument = (await operatorService.DownloadOperatorDocument(fileName));

        string downloadFileName = operatorDocument == null ? originalFileName : operatorDocument.OriginalFilename;

        var folder = configuration.AttachmentsPath;
        Directory.CreateDirectory(folder);
        var path = Path.Combine(folder, fileName);

        Stream stream = System.IO.File.OpenRead(path);
        return File(stream, mimeTypeProvider.Provide(fileName),downloadFileName);
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
