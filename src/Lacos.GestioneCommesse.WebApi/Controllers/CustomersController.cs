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

namespace Lacos.GestioneCommesse.WebApi.Controllers;

[RequireUser]
public class CustomersController : LacosApiController
{
    private readonly IContactService contactService;

    public CustomersController(
        IContactService contactService)
    {
        this.contactService = contactService;
    }

    [HttpGet("customers")]
    public async Task<ActionResult<DataSourceResult>> GetCustomers([DataSourceRequest] DataSourceRequest request)
    {
        var contacts = await contactService.GetContacts(ContactType.Customer);
        return Ok(await contacts.ToDataSourceResultAsync(request));
    }

    [HttpGet("providers")]
    public async Task<ActionResult<DataSourceResult>> GetProviders([DataSourceRequest] DataSourceRequest request)
    {
        var contacts = await contactService.GetContacts(ContactType.Supplier);
        return Ok(await contacts.ToDataSourceResultAsync(request));
    }

    [HttpGet("customer/{id}")]
    public async Task<ActionResult<ContactDto>> GetCustomer(long id)
    {
        var contact = await contactService.GetContact(id);
        return Ok(contact);
    }

    [HttpPut("customer/{id}")]
    public async Task<IActionResult> UpdateCustomer(long id, [FromBody] ContactDto request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        
        await contactService.UpdateContact(id, request);
        return Ok();
    }

    [HttpDelete("customer/{id}")]
    public async Task<IActionResult> DeleteCustomer(long id)
    {
        await contactService.DeleteContact(id);
        return Ok();
    }

    [HttpPost("customer")]
    public async Task<IActionResult> CreateCustomer([FromBody] ContactDto request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        
        var contact = await contactService.CreateContact(request);
        return Ok(contact);
    }
}