using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Lacos.GestioneCommesse.Application.Docs.DTOs;
using Lacos.GestioneCommesse.Application.Docs.Services;
using Lacos.GestioneCommesse.Application.Products.Service;
using Lacos.GestioneCommesse.Framework.Configuration;
using Lacos.GestioneCommesse.Framework.IO;
using Lacos.GestioneCommesse.WebApi.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Configuration;

namespace Lacos.GestioneCommesse.WebApi.Controllers;

[RequireUser]
public class InterventionsController : LacosApiController
{
    private readonly IInterventionsService service;
    private readonly ILacosConfiguration configuration;
    private readonly IMimeTypeProvider mimeTypeProvider;

    public InterventionsController(IInterventionsService service, ILacosConfiguration configuration, IMimeTypeProvider mimeTypeProvider)
    {
        this.service = service;
        this.configuration = configuration;
        this.mimeTypeProvider = mimeTypeProvider;
    }

    [HttpGet("read")]
    public Task<DataSourceResult> Read(DataSourceRequest request)
    {
        return service.Query()
            .ToDataSourceResultAsync(request);
    }

    [HttpGet("{id}")]
    public Task<InterventionDto> Get(long id)
    {
        return service.Get(id);
    }

    [HttpPost]
    public Task<InterventionDto> Create(InterventionDto interventionDto)
    {
        return service.Create(interventionDto);
    }

    [HttpPut("{id}")]
    public Task<InterventionDto> Update(long id, InterventionDto interventionDto)
    {
        interventionDto.Id = id;

        return service.Update(interventionDto);
    }

    [HttpDelete("{id}")]
    public Task Delete(long id)
    {
        return service.Delete(id);
    }

    [HttpGet("intervention-checklist-by-product/{id}")]
    public Task<InterventionProductCheckListDto> GetCheckListItemsByProduct(long id)
    {
        return service.GetInterventionProductCheckList(id);
    }

    [HttpGet("intervention-products-by-intervention/{id}")]
    public async Task<DataSourceResult> GetInterventionProductsByIntervention([DataSourceRequest] DataSourceRequest request, long id)
    {
        var products = (service.GetProductsByIntervention(id));
        return await products.ToDataSourceResultAsync(request);
    }

    [HttpGet("download-report/{interventionId}")]
    public async Task<FileResult> DownloadReport(long interventionId)
    {
        var report = await service.GenerateReport(interventionId);
        return File(report.Content, "application/pdf", report.FileName);
    }

    [AllowAnonymous]
    [HttpGet("intervention-note/download-file/{fileName}")]
    public async Task<FileResult> DownloadAttachment(string fileName)
    {
        fileName = Uri.UnescapeDataString(fileName);

        var activityAttachment = await service.DownloadInterventionNote(fileName);
        var downloadFileName = activityAttachment == null
            ? fileName
            : activityAttachment.PictureFileName;
        var folder = configuration.AttachmentsPath!;

        if (!Directory.Exists(folder))
        {
            Directory.CreateDirectory(folder);
        }

        var path = Path.Combine(folder, fileName);
        var mimeType = mimeTypeProvider.Provide(fileName);
        var stream = System.IO.File.OpenRead(path);

        return File(stream, mimeType, downloadFileName);
    }

    [HttpGet("all-attachments/{jobId}/{activityId}")]
    public async Task<List<InterventionNoteDto>> GetInterventionAttachments(long jobId, long activityId)
    {
        List<InterventionNoteDto> activitysAttachment = (await service.GetInterventionAttachments(jobId, activityId)).ToList();
        return activitysAttachment;
    }

    [HttpGet("interventions-ko")]
    public Task<DataSourceResult> ReadInterventionsKo(DataSourceRequest request)
    {
        return service.GetInterventionsKo()
            .ToDataSourceResultAsync(request);
    }

}