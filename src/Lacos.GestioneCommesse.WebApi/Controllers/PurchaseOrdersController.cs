using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Lacos.GestioneCommesse.Application.Docs.DTOs;
using Lacos.GestioneCommesse.Application.Docs.Services;
using Lacos.GestioneCommesse.Domain.Docs;
using Lacos.GestioneCommesse.Framework.Configuration;
using Lacos.GestioneCommesse.Framework.IO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Configuration;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Lacos.GestioneCommesse.WebApi.Controllers;

[Route("api/purchase-orders")]
public class PurchaseOrdersController : LacosApiController
{
    private readonly IPurchaseOrdersService service;
    private readonly ILacosConfiguration configuration;
    private readonly IMimeTypeProvider mimeTypeProvider;

    public PurchaseOrdersController(IPurchaseOrdersService service, ILacosConfiguration configuration, IMimeTypeProvider mimeTypeProvider)
    {
        this.service = service;
        this.configuration = configuration;
        this.mimeTypeProvider = mimeTypeProvider;
    }

    [HttpGet("read/{jobId?}")]
    public Task<DataSourceResult> Read(DataSourceRequest request, long? jobId)
    {
        return service.Query(jobId)
            .ToDataSourceResultAsync(request);
    }

    [HttpGet("{id}")]
    public Task<PurchaseOrderDto> Get(long id)
    {
        return service.Get(id);
    }

    [HttpPost]
    public Task<PurchaseOrderDto> Create(PurchaseOrderDto purchaseOrderDto)
    {
        return service.Create(purchaseOrderDto);
    }

    [HttpPut("{id}")]
    public Task<PurchaseOrderDto> Update(long id, PurchaseOrderDto purchaseOrderDto)
    {
        purchaseOrderDto.Id = id;

        return service.Update(purchaseOrderDto);
    }

    [HttpDelete("{id}")]
    public Task Delete(long id)
    {
        return service.Delete(id);
    }

    [HttpGet("job-purchaseOrders-dependencies/{jobId}")]
    public Task<DataSourceResult> GetJobActivities([DataSourceRequest] DataSourceRequest request, long jobId)
    {
        return service.GetJobPurchaseOrders(jobId)
            .ToDataSourceResultAsync(request);
    }

    // attachments ---------------------------------------------------------------------

    [HttpPost("create-attachment")]
    public async Task<IActionResult> CreatePurchaseOrderAttachment([FromBody] PurchaseOrderAttachmentDto purchaseOrderAttachmentDtoDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        await service.CreatePurchaseOrderAttachment(purchaseOrderAttachmentDtoDto);
        return Ok(purchaseOrderAttachmentDtoDto);
    }

    [HttpPut("update-attachment/{id}")]
    public async Task<IActionResult> UpdatePurchaseOrderAttachment(long id, [FromBody] PurchaseOrderAttachmentDto purchaseOrderAttachmentDtoDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        await service.UpdatePurchaseOrderAttachment(id, purchaseOrderAttachmentDtoDto);
        return Ok();
    }

    [AllowAnonymous]
    [HttpGet("purchase-order-attachment/download-file/{fileName}/{originalFileName}")]
    public async Task<FileResult> DownloadAttachment(string fileName, string originalFileName)
    {
        fileName = Uri.UnescapeDataString(fileName);
        originalFileName = Uri.UnescapeDataString(originalFileName);

        var purchaseOrderAttachment = await service.DownloadPurchaseOrderAttachment(fileName);
        var downloadFileName = purchaseOrderAttachment == null
            ? originalFileName
            : purchaseOrderAttachment.DisplayName;
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

    [HttpPost("purchase-order-attachment/upload-file")]
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

    [HttpPost("purchase-order-attachment/remove-file")]
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

    [HttpGet("all-attachments/{jobId}/{purchaseOrderId}")]
    public async Task<List<PurchaseOrderAttachmentReadModel>> GetPurchaseOrderAttachments(long jobId, long purchaseOrderId)
    {
        List<PurchaseOrderAttachmentReadModel> purchaseOrdersAttachment = (await service.GetPurchaseOrderAttachments(jobId,purchaseOrderId)).Where(e => !e.IsAdminDocument).ToList();
        return purchaseOrdersAttachment;
    }

    [HttpGet("all-admin-attachments/{jobId}/{purchaseOrderId}")]
    public async Task<List<PurchaseOrderAttachmentReadModel>> GetPurchaseOrderAdminAttachments(long jobId, long purchaseOrderId)
    {
        List<PurchaseOrderAttachmentReadModel> purchaseOrdersAttachment = (await service.GetPurchaseOrderAttachments(jobId, purchaseOrderId)).Where(e => e.IsAdminDocument).ToList();
        return purchaseOrdersAttachment;
    }

    [HttpPost("copy-purchase-order")]
    public Task<PurchaseOrderDto> CopyPurchaseOrder(CopyDto copyDto)
    {
        return service.CopyPurchaseOrder(copyDto);
    }
}