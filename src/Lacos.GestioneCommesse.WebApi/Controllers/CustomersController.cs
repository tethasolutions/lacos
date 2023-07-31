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

    [HttpGet("customers-list")]
    public async Task<ActionResult<List<ContactDto>>> GetCustomersList()
    {
        var contacts = new List<ContactDto>
        {
            new ContactDto
            {
                Id = 1,
                Type = ContactType.Customer,
                CompanyName = "General Motors",
                Name = "Mario",
                Surname = "Rossi",
                FiscalType = ContactFiscalType.PrivatePerson,
                ErpCode = "ABC",
                Alert = false,
                Addresses = new List<AddressDto>
                {
                    new AddressDto
                    {
                        Id = 1,
                        ContactId = 1,
                        City = "Bergamo",
                        StreetAddress = "via Zelasco 12",
                        Province = "BG",
                        ZipCode = "24012",
                        Telephone = "+39832471924",
                        Email = "mario@rossi.com"
                    },
                    new AddressDto
                    {
                        Id = 2,
                        ContactId = 1,
                        City = "Milano",
                        StreetAddress = "via Volta 17",
                        Province = "MI",
                        ZipCode = "29018",
                        Telephone = "+3983674352",
                        Email = "mario@rossi.it"
                    }
                }
            },
            new ContactDto
            {
                Id = 2,
                Type = ContactType.Customer,
                CompanyName = "McDonalds",
                Name = "Fernando",
                Surname = "Torrez",
                FiscalType = ContactFiscalType.PrivatePerson,
                ErpCode = "DGH",
                Alert = false,
                Addresses = new List<AddressDto>
                {
                    new AddressDto
                    {
                        Id = 3,
                        ContactId = 2,
                        City = "Bologna",
                        StreetAddress = "via Zelasco 14",
                        Province = "BL",
                        ZipCode = "24012",
                        Telephone = "+39832471924",
                        Email = "fernando@torrez.com"
                    },
                    new AddressDto
                    {
                        Id = 4,
                        ContactId = 2,
                        City = "Savona",
                        StreetAddress = "via Volta 19",
                        Province = "SV",
                        ZipCode = "29018",
                        Telephone = "+3983674352",
                        Email = "fernando@torrez.it"
                    }
                }
            },
            new ContactDto
            {
                Id = 3,
                Type = ContactType.Customer,
                CompanyName = "Google",
                Name = "Daniele",
                Surname = "Valtemara",
                FiscalType = ContactFiscalType.PrivatePerson,
                ErpCode = "KLD",
                Alert = false,
                Addresses = new List<AddressDto>
                {
                    new AddressDto
                    {
                        Id = 5,
                        ContactId = 3,
                        City = "Genova",
                        StreetAddress = "via Zelasco 23",
                        Province = "GN",
                        ZipCode = "28012",
                        Telephone = "+39832471924",
                        Email = "daniele@valtemara.com"
                    },
                    new AddressDto
                    {
                        Id = 6,
                        ContactId = 3,
                        City = "Venezia",
                        StreetAddress = "via Volta 34",
                        Province = "VN",
                        ZipCode = "21018",
                        Telephone = "+3983674352",
                        Email = "daniele@valtemara.it"
                    }
                }
            }
        };
        return Ok(contacts);
    }
}