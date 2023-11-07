using Lacos.GestioneCommesse.Application.Suppliers.DTOs;
using Lacos.GestioneCommesse.Application.Suppliers.Services;
using Lacos.GestioneCommesse.Application.Security;
using Lacos.GestioneCommesse.Application.Security.DTOs;
using Lacos.GestioneCommesse.WebApi.Auth;
using Lacos.GestioneCommesse.WebApi.Models.Security;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Lacos.GestioneCommesse.WebApi.Controllers;

[RequireUser]
public class AddressesSupplierController : LacosApiController
{
    private readonly IAddressSupplierService addressSupplierService;

    public AddressesSupplierController(
        IAddressSupplierService addressSupplierService)
    {
        this.addressSupplierService = addressSupplierService;
    }

    [HttpGet("address/customer/{id}")]
    public async Task<ActionResult<AddressDto>> GetAddresses(long id)
    {
        var addresses = await addressSupplierService.GetAddresses(id);

        return Ok(addresses);
    }

    [HttpGet("address/{id}")]
    public async Task<ActionResult<AddressDto>> GetAddress(long id)
    {
        var address = await addressSupplierService.GetAddress(id);

        return Ok(address);
    }

    [HttpPut("address/{id}")]
    public async Task<IActionResult> UpdateAddress(long id, [FromBody] AddressDto request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        
        var address = await addressSupplierService.UpdateAddress(id, request);

        return Ok(address);
    }

    [HttpDelete("address/{id}")]
    public async Task<IActionResult> DeleteAddress(long id)
    {
        await addressSupplierService.DeleteAddress(id);

        return Ok();
    }

    [HttpPost("address")]
    public async Task<IActionResult> CreateAddress([FromBody] AddressDto request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        
        var address = await addressSupplierService.CreateAddress(request);

        return Ok(address);
    }

    [HttpPut("set-address-as-main/{id}")]
    public async Task<IActionResult> SetAddressAsMain(long id)
    {
        var address = await addressSupplierService.SetMainAddress(id);

        return Ok(address);
    }
}