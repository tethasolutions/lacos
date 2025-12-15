using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Lacos.GestioneCommesse.Application.Customers.Services;
using Lacos.GestioneCommesse.Application.Registry.DTOs;
using Lacos.GestioneCommesse.Application.Registry.Services;
using Lacos.GestioneCommesse.Application.Security;
using Lacos.GestioneCommesse.Application.Security.DTOs;
using Lacos.GestioneCommesse.WebApi.Auth;
using Lacos.GestioneCommesse.WebApi.Models.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Lacos.GestioneCommesse.WebApi.Controllers;

[RequireUser]
public class AddressesController : LacosApiController
{
    private readonly IAddressService addressService;

    public AddressesController(
        IAddressService addressService)
    {
        this.addressService = addressService;
    }

    [HttpGet("address/customer/{id}")]
    public async Task<ActionResult<AddressDto>> GetCustomerAddresses(long id)
    {
        var addresses = await addressService.GetCustomerAddresses(id);

        return Ok(addresses);
    }

    [HttpGet("address/supplier/{id}")]
    public async Task<ActionResult<AddressDto>> GetSupplierAddresses(long id)
    {
        var addresses = await addressService.GetSupplierAddresses(id);

        return Ok(addresses);
    }

    [HttpGet("addresses")]
    public async Task<ActionResult<AddressDto>> GetAddresses()
    {
        var addresses = await addressService.GetAddresses();

        return Ok(addresses);
    }

    [HttpGet("address/{id}")]
    public async Task<ActionResult<AddressDto>> GetAddress(long id)
    {
        var address = await addressService.GetAddress(id);

        return Ok(address);
    }

    [HttpPut("address/{id}")]
    public async Task<IActionResult> UpdateAddress(long id, [FromBody] AddressDto request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        
        var address = await addressService.UpdateAddress(id, request);

        return Ok(address);
    }

    [HttpDelete("address/{id}")]
    public async Task<IActionResult> DeleteAddress(long id)
    {
        await addressService.DeleteAddress(id);

        return Ok();
    }

    [HttpPost("address")]
    public async Task<IActionResult> CreateAddress([FromBody] AddressDto request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        
        var address = await addressService.CreateAddress(request);

        return Ok(address);
    }

    [HttpPut("sync-distances")]
    public async Task<IActionResult> SyncDistances()
    {
        await addressService.SyncDistances(null,null,true);

        return Ok();
    }

    [HttpPut("set-address-as-main/{id}")]
    public async Task<IActionResult> SetAddressAsMain(long id)
    {
        var address = await addressService.SetMainAddress(id);

        return Ok(address);
    }

    [HttpGet("distance-errors")]
    public async Task<ActionResult<DataSourceResult>> GetDistanceErrors([DataSourceRequest] DataSourceRequest request)
    {
        var customers = await addressService.GetDistanceErrors();
        return Ok(await customers.ToDataSourceResultAsync(request));
    }

}