using Lacos.GestioneCommesse.WebApi.Auth;
using Microsoft.AspNetCore.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
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

    [HttpGet("activitytypes-list")]
    public async Task<IEnumerable<ActivityTypeDto>> GetActivityTypesList()
    {
        return await activityTypeService.GetActivityTypes();
    }

    [HttpGet("activitytypes-list-po")]
    public async Task<IEnumerable<ActivityTypeDto>> GetActivityTypesListPO()
    {
        return await activityTypeService.GetActivityTypes(true);
    }

    [HttpGet("activitytypes-list-calendar")]
    public async Task<IEnumerable<ActivityTypeDto>> GetActivityTypesListCalendar()
    {
        var ActivityTypes = await activityTypeService.GetActivityTypes();
        return ActivityTypes.Where(x => x.IsExternal == false && x.IsInternal == false);
    }
}
