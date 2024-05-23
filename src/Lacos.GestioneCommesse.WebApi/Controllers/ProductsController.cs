using Lacos.GestioneCommesse.WebApi.Auth;
using Microsoft.AspNetCore.Mvc;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using Lacos.GestioneCommesse.Application.Products.DTOs;
using Lacos.GestioneCommesse.Application.Products.Service;
using Lacos.GestioneCommesse.Application.Registry.DTOs;
using Lacos.GestioneCommesse.Framework.Configuration;
using Lacos.GestioneCommesse.Framework.IO;
using Microsoft.AspNetCore.Authorization;

namespace Lacos.GestioneCommesse.WebApi.Controllers;

[RequireUser]
public class ProductsController : LacosApiController
{

    private readonly IProductService productService;
    private readonly ILacosConfiguration configuration;
    private readonly IMimeTypeProvider mimeTypeProvider;

    public ProductsController(IProductService productService, ILacosConfiguration configuration, IMimeTypeProvider mimeTypeProvider)
    {
        this.productService = productService;
        this.configuration = configuration;
        this.mimeTypeProvider = mimeTypeProvider;
    }

    [HttpGet("products")]
    public async Task<DataSourceResult> GetProducts([DataSourceRequest] DataSourceRequest request)
    {
        var products = (productService.GetProducts());
        return await products.ToDataSourceResultAsync(request);
    }

    [HttpGet("products/spare-parts")]
    public async Task<DataSourceResult> GetSpareParts([DataSourceRequest] DataSourceRequest request)
    {
        var products = (productService.GetSpareParts());
        return await products.ToDataSourceResultAsync(request);
    }

    [HttpGet("product-detail/{productId}")]
    public async Task<ProductDto> GetProductDetail(long productId)
    {
        var product = await productService.GetProductDetail(productId);
        return product;
    }

    [HttpPut("product/{id}")]
    public async Task<IActionResult> UpdateProduct(long id, [FromBody] ProductDto productDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        await productService.UpdateProduct(id, productDto);
        return Ok();
    }

    [HttpPost("product")]
    public async Task<IActionResult> CreateProduct( [FromBody] ProductDto productDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        await productService.CreateProduct(productDto);
        return Ok(productDto);
    }

    [HttpDelete("product/{id}")]
    public async Task<IActionResult> DeleteProduct(long productId)
    {
        await productService.DeleteProduct(productId);
        return Ok();
    }

    [HttpGet("product-types")]
    public async Task<List<ProductTypeDto>> GetProductTypes()
    {
        List<ProductTypeDto> productTypes = (await productService.GetProductTypes()).ToList();
        return productTypes;
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

    [AllowAnonymous]
    [HttpGet("product-document/download-file/{fileName}/{originalFileName}")]
    public async Task<FileResult> DownloadAttachment(string fileName, string originalFileName)
    {
        fileName = Uri.UnescapeDataString(fileName);
        originalFileName = Uri.UnescapeDataString(originalFileName);

        var productDocument = await productService.DownloadProductDocument(fileName);
        var downloadFileName = productDocument == null
            ? originalFileName
            : productDocument.OriginalFilename;
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
