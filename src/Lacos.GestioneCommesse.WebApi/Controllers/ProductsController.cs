using Lacos.GestioneCommesse.WebApi.Auth;
using Microsoft.AspNetCore.Mvc;
using Lacos.GestioneCommesse.Application.Vehicles.DTOs;
using Kendo.Mvc.UI;
using Lacos.GestioneCommesse.Domain.Docs;
using Lacos.GestioneCommesse.Application.Operators.DTOs;
using System.Net.Http;
using System.Web.Http;
using Kendo.Mvc.Extensions;
using Lacos.GestioneCommesse.Domain.Registry;
using Telerik.SvgIcons;
using Lacos.GestioneCommesse.Application.Products.DTOs;
using Lacos.GestioneCommesse.Application.Products.Service;
using Lacos.GestioneCommesse.Application.Registry.DTOs;

namespace Lacos.GestioneCommesse.WebApi.Controllers;

[RequireUser]
public class ProductsController : LacosApiController
{

    private readonly IProductService productService;

    [HttpGet("products")]
    public async Task<DataSourceResult> GetProducts([DataSourceRequest] DataSourceRequest request)
    {
        var products = (productService.GetProducts());
        return await products.ToDataSourceResultAsync(request);
    }

    [HttpGet("product-detail/{productId}")]
    public async Task<ProductReadModel> GetProductDetail(long productId)
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

    [HttpGet("product-qr-code/{productId}")]
    public async Task<string> CreateProductQrCode(long productId)
    {
        string qrCode = await productService.CreateProductQrCode(productId);
        return qrCode;
    }
}
