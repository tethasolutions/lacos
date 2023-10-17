using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Lacos.GestioneCommesse.Application.Docs.DTOs;
using Lacos.GestioneCommesse.Application.Docs.Services;
using Lacos.GestioneCommesse.WebApi.ModelBinders;
using Microsoft.AspNetCore.Mvc;

namespace Lacos.GestioneCommesse.WebApi.Controllers;

[Route("api/activity-products")]
public class ActivityProductsController : LacosApiController
{
    private readonly IActivityProductsService service;

    public ActivityProductsController(IActivityProductsService service)
    {
        this.service = service;
    }

    [HttpGet("read")]
    public async Task<DataSourceResult> Read([LacosDataSourceRequest] DataSourceRequest request)
    {
        return await service.Query()
            .ToDataSourceResultAsync(request);
    }

    [HttpPost]
    public Task Create(ActivityProductDto activityProductDto)
    {
        return service.Create(activityProductDto);
    }

    [HttpPost("{id}/duplicate")]
    public Task Duplicate(long id)
    {
        return service.Duplicate(id);
    }

    [HttpDelete("{id}")]
    public Task Delete(long id)
    {
        return service.Delete(id);
    }
}