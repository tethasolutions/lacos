using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Lacos.GestioneCommesse.Application.Docs.DTOs;
using Lacos.GestioneCommesse.Application.Docs.Services;
using Lacos.GestioneCommesse.Framework.Configuration;
using Lacos.GestioneCommesse.Framework.IO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace Lacos.GestioneCommesse.WebApi.Controllers;

public class ActivitiesController : LacosApiController
{
    private readonly IActivitiesService service;
    private readonly ILacosConfiguration configuration;
    private readonly IMimeTypeProvider mimeTypeProvider;

    public ActivitiesController(IActivitiesService service, ILacosConfiguration configuration, IMimeTypeProvider mimeTypeProvider)
    {
        this.service = service;
        this.configuration = configuration;
        this.mimeTypeProvider = mimeTypeProvider;
    }

    [HttpGet("read")]
    public Task<DataSourceResult> Read([DataSourceRequest] DataSourceRequest request)
    {
        return service.Query()
            .ToDataSourceResultAsync(request);
    }

    [HttpGet("{product}/activitiesFromProduct")]
    public Task<DataSourceResult> activitiesFromProducts([DataSourceRequest] DataSourceRequest request, string product)
    {
        return service.GetActivitiesFromProduct(product)
            .ToDataSourceResultAsync(request);
    }


    [HttpGet("{id}")]
    public Task<ActivityDto> Get(long id)
    {
        return service.Get(id);
    }

    [HttpGet("{id}/detail")]
    public Task<ActivityDetailDto> GetDetail(long id)
    {
        return service.GetDetail(id);
    }

    [HttpPost]
    public Task<ActivityDto> Create(ActivityDto activityDto)
    {
        return service.Create(activityDto);
    }

    [HttpPut("{id}")]
    public Task<ActivityDto> Update(long id, ActivityDto activityDto)
    {
        activityDto.Id = id;

        return service.Update(activityDto);
    }

    [HttpDelete("{id}")]
    public Task Delete(long id)
    {
        return service.Delete(id);
    }

    [HttpPut("{id}/assign-all-customer-products")]
    public Task AssignAllCustomerProducts(long id)
    {
        return service.AssignAllCustomerProducts(id);
    }

    [HttpGet("activities-counters")]
    public async Task<IEnumerable<ActivityCounterDto>> GetActivitiesCounters()
    {
        return await service.GetActivitiesCounters();
    }

    [HttpGet("new-activities-counter")]
    public async Task<ActivityCounterNewDto> GetNewActivitiesCounters()
    {
        return await service.GetNewActivitiesCounter();
    }

    [HttpGet("attachment-detail/{id}")]
    public async Task<ActivityAttachmentReadModel> GetAttachmentDetail(long id)
    {
        var activity = await service.GetActivityAttachmentDetail(id);
        return activity;
    }

    [HttpPost("create-attachment")]
    public async Task<IActionResult> CreateActivityAttachment([FromBody] ActivityAttachmentDto activityAttachmentDtoDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        await service.CreateActivityAttachment(activityAttachmentDtoDto);
        return Ok(activityAttachmentDtoDto);
    }

    [HttpPut("update-attachment/{id}")]
    public async Task<IActionResult> UpdateActivityAttachment(long id, [FromBody] ActivityAttachmentDto activityAttachmentDtoDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        await service.UpdateActivityAttachment(id, activityAttachmentDtoDto);
        return Ok();
    }

    [HttpGet("all-attachments/{jobId}/{activityId}")]
    public async Task<List<ActivityAttachmentReadModel>> GetActivityAttachments(long jobId, long activityId)
    {
        List<ActivityAttachmentReadModel> activitysAttachment = (await service.GetActivityAttachments(jobId, activityId)).ToList();
        return activitysAttachment;
    }

    [AllowAnonymous]
    [HttpGet("activity-attachment/download-file/{fileName}/{originalFileName}")]
    public async Task<FileResult> DownloadAttachment(string fileName, string originalFileName)
    {
        fileName = Uri.UnescapeDataString(fileName);
        originalFileName = Uri.UnescapeDataString(originalFileName);

        var activityAttachment = await service.DownloadActivityAttachment(fileName);
        var downloadFileName = activityAttachment == null 
            ? originalFileName
            : activityAttachment.DisplayName;
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

    [HttpPost("activity-attachment/upload-file")]
    public async Task<IActionResult> UploadFile()
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

    [HttpPost("activity-attachment/remove-file")]
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