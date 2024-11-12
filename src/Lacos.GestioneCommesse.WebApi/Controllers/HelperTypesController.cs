using Lacos.GestioneCommesse.WebApi.Auth;
using Microsoft.AspNetCore.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Lacos.GestioneCommesse.Application.Registry.DTOs;
using Lacos.GestioneCommesse.Application.Registry.Services;

namespace Lacos.GestioneCommesse.WebApi.Controllers;

[RequireUser]
public class HelperTypesController : LacosApiController
{
    private readonly IHelperTypeService helperTypeService;

    public HelperTypesController(IHelperTypeService helperTypeService)
    {
        this.helperTypeService = helperTypeService;
    }

    [HttpGet()]
    public async Task<ActionResult<HelperTypeDto>> GetHelperTypes([DataSourceRequest] DataSourceRequest request)
    {
        var helperTypes = await helperTypeService.GetHelperTypes();
        return Ok(await helperTypes.ToDataSourceResultAsync(request));
    }

    [HttpGet("{id}")]
    public async Task<HelperTypeDto> GetHelperTypeDetail(long id)
    {
        var helperType = await helperTypeService.GetHelperType(id);
        return helperType;
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateHelperType(long id, [FromBody] HelperTypeDto helperTypeDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        await helperTypeService.UpdateHelperType(id, helperTypeDto);
        return Ok();
    }

    [HttpPost()]
    public async Task<IActionResult> CreateHelperType([FromBody] HelperTypeDto helperTypeDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        await helperTypeService.CreateHelperType(helperTypeDto);
        return Ok(helperTypeDto);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteHelperType(long id)
    {
        await helperTypeService.DeleteHelperType(id);

        return Ok();
    }

}
