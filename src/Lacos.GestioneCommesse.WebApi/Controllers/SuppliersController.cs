using Lacos.GestioneCommesse.Application.Suppliers.DTOs;
using Lacos.GestioneCommesse.Application.Suppliers.Services;
using Lacos.GestioneCommesse.Application.Security;
using Lacos.GestioneCommesse.Application.Security.DTOs;
using Lacos.GestioneCommesse.Domain.Registry;
using Lacos.GestioneCommesse.WebApi.Auth;
using Lacos.GestioneCommesse.WebApi.Models.Security;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Reflection.Emit;

namespace Lacos.GestioneCommesse.WebApi.Controllers;

[RequireUser]
public class SuppliersController : LacosApiController
{
    private readonly ISupplierService supplierService;

    public SuppliersController(
        ISupplierService supplierService)
    {
        this.supplierService = supplierService;
    }

    [HttpGet("suppliers")]
    public async Task<ActionResult<DataSourceResult>> GetSuppliers([DataSourceRequest] DataSourceRequest request)
    {
        var suppliers = await supplierService.GetSuppliers();
        return Ok(await suppliers.ToDataSourceResultAsync(request));
    }

    [HttpGet("supplier/{id}")]
    public async Task<ActionResult<SupplierDto>> GetSupplier(long id)
    {
        var supplier = await supplierService.GetSupplier(id);
        return Ok(supplier);
    }

    [HttpPut("supplier/{id}")]
    public async Task<IActionResult> UpdateSupplier(long id, [FromBody] SupplierDto request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        
        await supplierService.UpdateSupplier(id, request);
        return Ok();
    }

    [HttpDelete("supplier/{id}")]
    public async Task<IActionResult> DeleteSupplier(long id)
    {
        await supplierService.DeleteSupplier(id);
        return Ok();
    }

    [HttpPost("supplier")]
    public async Task<IActionResult> CreateSupplier([FromBody] SupplierDto request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        
        var supplier = await supplierService.CreateSupplier(request);
        return Ok(supplier);
    }

    [HttpGet("suppliers-list")]
    public async Task<ActionResult<List<SupplierDto>>> GetSuppliersList()
    {
        var suppliers = (await supplierService.GetSuppliers()).ToList();
        return Ok(suppliers);
    }
}