using Lacos.GestioneCommesse.WebApi.Auth;
using Microsoft.AspNetCore.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Lacos.GestioneCommesse.Application.Registry.DTOs;
using Lacos.GestioneCommesse.Application.Registry.Services;
using Lacos.GestioneCommesse.Application.Products.Service;
using Lacos.GestioneCommesse.Framework.IO;
using Microsoft.AspNetCore.Authorization;
using System.Configuration;
using Lacos.GestioneCommesse.Framework.Configuration;

namespace Lacos.GestioneCommesse.WebApi.Controllers;

[RequireUser]
public class HelperDocumentsController : LacosApiController
{
    private readonly IHelperDocumentService helperDocumentService;
    private readonly ILacosConfiguration configuration;

    public HelperDocumentsController(IHelperDocumentService helperDocumentService, ILacosConfiguration configuration)
    {
        this.helperDocumentService = helperDocumentService;
        this.configuration = configuration;
    }

    [HttpGet()]
    public async Task<ActionResult<HelperDocumentReadModel>> GetHelperDocuments([DataSourceRequest] DataSourceRequest request)
    {
        var helperDocuments = await helperDocumentService.GetHelperDocuments();
        return Ok(await helperDocuments.ToDataSourceResultAsync(request));
    }

    [HttpGet("{id}")]
    public async Task<HelperDocumentDto> GetHelperDocumentDetail(long id)
    {
        var helperDocument = await helperDocumentService.GetHelperDocument(id);
        return helperDocument;
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateHelperDocument(long id, [FromBody] HelperDocumentDto helperDocumentDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        await helperDocumentService.UpdateHelperDocument(id, helperDocumentDto);
        return Ok();
    }

    [HttpPost()]
    public async Task<IActionResult> CreateHelperDocument([FromBody] HelperDocumentDto helperDocumentDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        await helperDocumentService.CreateHelperDocument(helperDocumentDto);
        return Ok(helperDocumentDto);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteHelperDocument(long id)
    {
        await helperDocumentService.DeleteHelperDocument(id);

        return Ok();
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
