using Lacos.GestioneCommesse.WebApi.Auth;
using Microsoft.AspNetCore.Mvc;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using Lacos.GestioneCommesse.Application.Operators.DTOs;
using Lacos.GestioneCommesse.Application.NotificationOperators.Service;
using Lacos.GestioneCommesse.Framework.Configuration;
using Lacos.GestioneCommesse.Framework.IO;
using Microsoft.AspNetCore.Authorization;

namespace Lacos.GestioneCommesse.WebApi.Controllers;

[RequireUser]
public class NotificationOperatorsController : LacosApiController
{

    private readonly INotificationOperatorService notificationOperatorService;
    private readonly ILacosConfiguration configuration;
    private readonly IMimeTypeProvider mimeTypeProvider;

    public NotificationOperatorsController(INotificationOperatorService notificationOperatorService, ILacosConfiguration configuration, IMimeTypeProvider mimeTypeProvider)
    {
        this.notificationOperatorService = notificationOperatorService;
        this.configuration = configuration;
        this.mimeTypeProvider = mimeTypeProvider;
    }

    [HttpGet("notificationOperators")]
    public async Task<DataSourceResult> GetNotificationOperators([DataSourceRequest] DataSourceRequest request)
    {
        var notificationOperators = (notificationOperatorService.GetNotificationOperators());
        return await notificationOperators.ToDataSourceResultAsync(request);
    }

    [HttpGet("notificationOperator-detail/{notificationOperatorId}")]
    public async Task<NotificationOperatorDto> GetNotificationOperatorDetail(long notificationOperatorId)
    {
        var notificationOperator = await notificationOperatorService.GetNotificationOperatorDetail(notificationOperatorId);
        return notificationOperator;
    }

    [HttpPut("notificationOperator/{id}")]
    public async Task<IActionResult> UpdateNotificationOperator(long id, [FromBody] NotificationOperatorDto notificationOperatorDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        await notificationOperatorService.UpdateNotificationOperator(id, notificationOperatorDto);
        return Ok();
    }

    [HttpPost("notificationOperator")]
    public async Task<IActionResult> CreateNotificationOperator( [FromBody] NotificationOperatorDto notificationOperatorDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        await notificationOperatorService.CreateNotificationOperator(notificationOperatorDto);
        return Ok(notificationOperatorDto);
    }

    [HttpDelete("notificationOperator/{id}")]
    public async Task<IActionResult> DeleteNotificationOperator(long notificationOperatorId)
    {
        await notificationOperatorService.DeleteNotificationOperator(notificationOperatorId);
        return Ok();
    }

}
