using Lacos.GestioneCommesse.Application.Customers.DTOs;
using Lacos.GestioneCommesse.Application.Customers.Services;
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
public class CustomersController : LacosApiController
{
    private readonly ICustomerService customerService;

    public CustomersController(
        ICustomerService customerService)
    {
        this.customerService = customerService;
    }

    [HttpGet("customers")]
    public async Task<ActionResult<DataSourceResult>> GetCustomers([DataSourceRequest] DataSourceRequest request)
    {
        var customers = await customerService.GetCustomers();
        return Ok(await customers.ToDataSourceResultAsync(request));
    }

    [HttpGet("customer/{id}")]
    public async Task<ActionResult<CustomerDto>> GetCustomer(long id)
    {
        var customer = await customerService.GetCustomer(id);
        return Ok(customer);
    }

    [HttpPut("customer/{id}")]
    public async Task<IActionResult> UpdateCustomer(long id, [FromBody] CustomerDto request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        
        await customerService.UpdateCustomer(id, request);
        return Ok();
    }

    [HttpDelete("customer/{id}")]
    public async Task<IActionResult> DeleteCustomer(long id)
    {
        await customerService.DeleteCustomer(id);
        return Ok();
    }

    [HttpPost("customer")]
    public async Task<IActionResult> CreateCustomer([FromBody] CustomerDto request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        
        var customer = await customerService.CreateCustomer(request);
        return Ok(customer);
    }

    [HttpGet("customers-list")]
    public async Task<ActionResult<List<CustomerDto>>> GetCustomersList()
    {
        var customers = (await customerService.GetCustomers()).ToList();
        return Ok(customers);
    }
}