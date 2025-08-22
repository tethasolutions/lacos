using Lacos.GestioneCommesse.WebApi.Auth;
using Microsoft.AspNetCore.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Lacos.GestioneCommesse.Application.Registry.DTOs;
using Lacos.GestioneCommesse.Application.Registry.Services;

namespace Lacos.GestioneCommesse.WebApi.Controllers;

[RequireUser]
public class AccountingTypesController : LacosApiController
{
    private readonly IAccountingTypeService accountingTypeService;

    public AccountingTypesController(IAccountingTypeService accountingTypeService)
    {
        this.accountingTypeService = accountingTypeService;
    }

    [HttpGet("accountingtypes")]
    public async Task<ActionResult<AccountingTypeDto>> GetAccountingTypes([DataSourceRequest] DataSourceRequest request)
    {
        var accountingTypes = await accountingTypeService.GetAccountingTypes();
        return Ok(await accountingTypes.ToDataSourceResultAsync(request));
    }

    [HttpGet("accountingtype-detail/{id}")]
    public async Task<AccountingTypeDto> GetAccountingTypeDetail(long id)
    {
        var accountingType = await accountingTypeService.GetAccountingType(id);
        return accountingType;
    }

    [HttpPut("accountingtype/{id}")]
    public async Task<IActionResult> UpdateAccountingType(long id, [FromBody] AccountingTypeDto accountingTypeDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        await accountingTypeService.UpdateAccountingType(id, accountingTypeDto);
        return Ok();
    }

    [HttpPost("accountingtype")]
    public async Task<IActionResult> CreateAccountingType([FromBody] AccountingTypeDto accountingTypeDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        await accountingTypeService.CreateAccountingType(accountingTypeDto);
        return Ok(accountingTypeDto);
    }

    [HttpDelete("accountingtype/{id}")]
    public async Task<IActionResult> DeleteAccountingType(long id)
    {
        return Ok();
    }

    [HttpGet("accountingtypes-list")]
    public async Task<IEnumerable<AccountingTypeDto>> GetAccountingTypesList()
    {
        return await accountingTypeService.GetAccountingTypes();
    }

}
