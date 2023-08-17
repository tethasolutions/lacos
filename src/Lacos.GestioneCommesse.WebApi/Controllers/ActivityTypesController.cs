using Lacos.GestioneCommesse.WebApi.Auth;
using Microsoft.AspNetCore.Mvc;
using Lacos.GestioneCommesse.Application.Vehicles.DTOs;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Lacos.GestioneCommesse.Domain.Docs;
using Lacos.GestioneCommesse.Application.Operators.DTOs;
using System.Net.Http;
using System.Web.Http;
using Lacos.GestioneCommesse.Application.CheckList.DTOs;
using Lacos.GestioneCommesse.Domain.Registry;
using Telerik.SvgIcons;
using Lacos.GestioneCommesse.Application.Registry.DTOs;
using Lacos.GestioneCommesse.Application.Registry.Services;

namespace Lacos.GestioneCommesse.WebApi.Controllers;

[RequireUser]
public class ActivityTypesController : LacosApiController
{
    private readonly IActivityTypeService activityTypeService;

    public ActivityTypesController(IActivityTypeService activityTypeService)
    {
        this.activityTypeService = activityTypeService;
    }

    [HttpGet("activitytypes")]
    public async Task<ActionResult<ActivityTypeDto>> GetActivityTypes([DataSourceRequest] DataSourceRequest request)
    {
        var activityTypes = await activityTypeService.GetActivityTypes();
        return Ok(await activityTypes.ToDataSourceResultAsync(request));
    }

    [HttpGet("activitytype-detail/{id}")]
    public async Task<ActivityTypeDto> GetActivityTypeDetail(long id)
    {
        var activityType = await activityTypeService.GetActivityType(id);
        return activityType;
    }

    [HttpPut("activitytype/{id}")]
    public async Task<IActionResult> UpdateActivityType(long id, [FromBody] ActivityTypeDto activityTypeDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        await activityTypeService.UpdateActivityType(id, activityTypeDto);
        return Ok();
    }

    [HttpPost("activitytype")]
    public async Task<IActionResult> CreateActivityType([FromBody] ActivityTypeDto activityTypeDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        await activityTypeService.CreateActivityType(activityTypeDto);
        return Ok(activityTypeDto);
    }

    [HttpDelete("activitytype/{id}")]
    public async Task<IActionResult> DeleteActivityType(long id)
    {
        return Ok();
    }

}
