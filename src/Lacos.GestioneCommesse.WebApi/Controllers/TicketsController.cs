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

public class TicketsController : LacosApiController
{
    private readonly ITicketsService service;
    private readonly ILacosConfiguration configuration;
    private readonly IMimeTypeProvider mimeTypeProvider;

    public TicketsController(ITicketsService service, ILacosConfiguration configuration, IMimeTypeProvider mimeTypeProvider)
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
    public Task<TicketDto> Get(long id)
    {
        return service.Get(id);
    }

    [HttpPost]
    public Task<TicketDto> Create(TicketDto ticketDto)
    {
        return service.Create(ticketDto);
    }

    [HttpPut("{id}")]
    public Task<TicketDto> Update(long id, TicketDto ticketDto)
    {
        ticketDto.Id = id;

        return service.Update(ticketDto);
    }

    [HttpDelete("{id}")]
    public Task Delete(long id)
    {
        return service.Delete(id);
    }

    [HttpGet("tickets-counters")]
    public async Task<TicketCounterDto> GetTicketsCounters()
    {
        return await service.GetTicketsCounters();
    }

    [HttpPost("create-attachment")]
    public async Task<IActionResult> CreateTicketAttachment([FromBody] TicketAttachmentDto ticketAttachmentDtoDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        await service.CreateTicketAttachment(ticketAttachmentDtoDto);
        return Ok(ticketAttachmentDtoDto);
    }

    [HttpPut("update-attachment/{id}")]
    public async Task<IActionResult> UpdateTicketAttachment(long id, [FromBody] TicketAttachmentDto ticketAttachmentDtoDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        await service.UpdateTicketAttachment(id, ticketAttachmentDtoDto);
        return Ok();
    }

    [AllowAnonymous]
    [HttpGet("ticket-attachment/download-file/{fileName}/{originalFileName}")]
    public async Task<FileResult> DownloadAttachment(string fileName, string originalFileName)
    {
        fileName = Uri.UnescapeDataString(fileName);
        originalFileName = Uri.UnescapeDataString(originalFileName);

        var ticketAttachment = await service.DownloadTicketAttachment(fileName);
        var downloadFileName = ticketAttachment == null
            ? originalFileName
            : ticketAttachment.Description;
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

    [HttpPost("ticket-attachment/upload-file")]
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

    [HttpPost("ticket-attachment/remove-file")]
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
    public async Task<List<TicketAttachmentReadModel>> GetTicketAttachments(long id)
    {
        List<TicketAttachmentReadModel> ticketsAttachment = (await service.GetTicketAttachments(id)).ToList();
        return ticketsAttachment;
    }
}