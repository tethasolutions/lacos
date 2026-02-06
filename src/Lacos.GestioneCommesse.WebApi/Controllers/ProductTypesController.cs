using Lacos.GestioneCommesse.WebApi.Auth;
using Microsoft.AspNetCore.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Lacos.GestioneCommesse.Application.Registry.DTOs;
using Lacos.GestioneCommesse.Application.Registry.Services;

namespace Lacos.GestioneCommesse.WebApi.Controllers;

[RequireUser]
public class ProductTypesController : LacosApiController
{
    private readonly IProductTypeService productTypeService;

    public ProductTypesController(IProductTypeService productTypeService)
    {
        this.productTypeService = productTypeService;
    }

    [HttpGet("producttypes")]
    public async Task<ActionResult<ProductTypeDto>> GetProductTypes([DataSourceRequest] DataSourceRequest request)
    {
        var productTypes = await productTypeService.GetProductTypes();
        return Ok(await productTypes.ToDataSourceResultAsync(request));
    }

    [HttpGet("producttype-detail/{id}")]
    public async Task<ProductTypeDto> GetProductTypeDetail(long id)
    {
        var productType = await productTypeService.GetProductType(id);
        return productType;
    }

    [HttpPut("producttype/{id}")]
    public async Task<IActionResult> UpdateProductType(long id, [FromBody] ProductTypeDto productTypeDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        await productTypeService.UpdateProductType(id, productTypeDto);
        return Ok();
    }

    [HttpPost("producttype")]
    public async Task<IActionResult> CreateProductType([FromBody] ProductTypeDto productTypeDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        await productTypeService.CreateProductType(productTypeDto);
        return Ok(productTypeDto);
    }

    [HttpDelete("producttype/{id}")]
    public async Task<IActionResult> DeleteProductType(long id)
    {
        return Ok();
    }

    [HttpGet("producttypes-list")]
    public async Task<List<ProductTypeDto>> GetProductTypesList([DataSourceRequest] DataSourceRequest request)
    {
        var productTypes = (await productTypeService.GetProductTypes()).ToList();
        return productTypes;
    }
}
