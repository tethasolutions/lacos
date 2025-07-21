using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Lacos.GestioneCommesse.Application.Docs.DTOs;
using Lacos.GestioneCommesse.Application.Docs.Services;
using Lacos.GestioneCommesse.Framework.Configuration;
using Lacos.GestioneCommesse.Framework.IO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Configuration;

namespace Lacos.GestioneCommesse.WebApi.Controllers;

public class JobsController : LacosApiController
{
    private readonly IJobsService service;
    private readonly ILacosConfiguration configuration;
    private readonly IMimeTypeProvider mimeTypeProvider;

    public JobsController(IJobsService service, ILacosConfiguration configuration, IMimeTypeProvider mimeTypeProvider)
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

    [HttpGet("readcurrentjobs")]
    public Task<DataSourceResult> ReadCurrentJobs(DataSourceRequest request)
    {
        return service.QueryCurrentJobs()
            .ToDataSourceResultAsync(request);
    }

    [HttpGet("readarchivedjobs")]
    public Task<DataSourceResult> ReadArchivedJobs(DataSourceRequest request)
    {
        return service.QueryArchivedJobs()
            .ToDataSourceResultAsync(request);
    }

    [HttpGet("getTicketJob/{customerId}/{AddressId}/{ticketCode}")]
    public Task<JobDto> GetTicketJob(long CustomerId, long? AddressId, string TicketCode)
    {
        return service.GetTicketJob(CustomerId, AddressId, TicketCode);
    }

    [HttpGet("{id}")]
    public Task<JobDto> Get(long id)
    {
        return service.Get(id);
    }

    [HttpPost]
    public Task<JobDto> Create(JobDto jobDto)
    {
        return service.Create(jobDto);
    }

    [HttpPut("{id}")]
    public Task<JobDto> Update(long id, JobDto jobDto)
    {
        jobDto.Id = id;

        return service.Update(jobDto);
    }

    [HttpDelete("{id}")]
    public Task Delete(long id)
    {
        return service.Delete(id);
    }

    [HttpPost("copyJob")]
    public async Task<long> CopyJob(JobCopyDto jobCopyDto)
    {
        return await service.CopyJob(jobCopyDto);
    }

    [HttpPost("create-attachment")]
    public async Task<IActionResult> CreateJobAttachment([FromBody] JobAttachmentDto jobAttachmentDtoDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        await service.CreateJobAttachment(jobAttachmentDtoDto);
        return Ok(jobAttachmentDtoDto);
    }

    [HttpPut("update-attachment/{id}")]
    public async Task<IActionResult> UpdateJobAttachment(long id, [FromBody] JobAttachmentDto jobAttachmentDtoDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        await service.UpdateJobAttachment(id, jobAttachmentDtoDto);
        return Ok();
    }

    [AllowAnonymous]
    [HttpGet("job-attachment/download-file/{fileName}/{originalFileName}")]
    public async Task<FileResult> DownloadAttachment(string fileName, string originalFileName)
    {
        fileName = Uri.UnescapeDataString(fileName);
        originalFileName = Uri.UnescapeDataString(originalFileName);

        var jobAttachment = await service.DownloadJobAttachment(fileName);
        var downloadFileName = jobAttachment == null
            ? originalFileName
            : jobAttachment.DisplayName;
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

    [HttpPost("job-attachment/upload-file")]
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

    [HttpPost("job-attachment/remove-file")]
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

    [HttpGet("{id}/all-attachments")]
    public async Task<List<JobAttachmentReadModel>> GetJobAttachments(long id)
    {
        List<JobAttachmentReadModel> jobsAttachment = (await service.GetJobAttachments(id)).ToList();
        return jobsAttachment;
    }

    [HttpGet("get-jobs-progress-status")]
    public Task<DataSourceResult> GetMessagesList([DataSourceRequest] DataSourceRequest request)
    {
        return service.GetJobsProgressStatus()
            .ToDataSourceResultAsync(request);
    }
}