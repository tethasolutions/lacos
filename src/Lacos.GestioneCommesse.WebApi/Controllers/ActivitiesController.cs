using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Lacos.GestioneCommesse.Application.Docs.DTOs;
using Lacos.GestioneCommesse.Application.Docs.Services;
using Microsoft.AspNetCore.Mvc;

namespace Lacos.GestioneCommesse.WebApi.Controllers;

public class ActivitiesController : LacosApiController
{
    private readonly IActivitiesService service;

    public ActivitiesController(IActivitiesService service)
    {
        this.service = service;
    }

    [HttpGet("read")]
    public Task<DataSourceResult> Read([DataSourceRequest] DataSourceRequest request)
    {
        return service.Query()
            .ToDataSourceResultAsync(request);
    }

    [HttpGet("{id}")]
    public Task<ActivityDto> Get(long id)
    {
        return service.Get(id);
    }

    [HttpGet("{id}/detail")]
    public Task<ActivityDetailDto> GetDetail(long id)
    {
        return service.GetDetail(id);
    }

    [HttpPost]
    public Task<ActivityDto> Create(ActivityDto activityDto)
    {
        return service.Create(activityDto);
    }

    [HttpPut("{id}")]
    public Task<ActivityDto> Update(long id, ActivityDto activityDto)
    {
        activityDto.Id = id;

        return service.Update(activityDto);
    }

    [HttpDelete("{id}")]
    public Task Delete(long id)
    {
        return service.Delete(id);
    }
}