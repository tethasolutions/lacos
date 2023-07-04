using Lacos.GestioneCommesse.Application.Customers.DTOs;
using Lacos.GestioneCommesse.Application.Customers.Services;
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
public class AddressesController : LacosApiController
{
    private readonly IAddressService addressService;

    public AddressesController(
        IAddressService addressService)
    {
        this.addressService = addressService;
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

    [HttpPut("set-address-as-main/{id}")]
    public async Task<IActionResult> SetAddressAsMain(long id)
    {
        var address = await addressService.SetMainAddress(id);

        return Ok(address);
    }
}