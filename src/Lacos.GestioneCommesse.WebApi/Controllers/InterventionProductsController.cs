using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Lacos.GestioneCommesse.Application.Docs.DTOs;
using Lacos.GestioneCommesse.Application.Docs.Services;
using Microsoft.AspNetCore.Mvc;

namespace Lacos.GestioneCommesse.WebApi.Controllers;

[Route("api/intervention-products")]
public class InterventionProductsController : LacosApiController
{
    private readonly IInterventionProductsService service;

    public InterventionProductsController(IInterventionProductsService service)
    {
        this.service = service;
    }

    [HttpGet("read")]
    public Task<DataSourceResult> Read([DataSourceRequest] DataSourceRequest request)
    {
        return service.Query()
            .ToDataSourceResultAsync(request);
    }

    [HttpPost]
    public Task Create(InterventionProductDto interventionProductDto)
    {
        return service.Create(interventionProductDto);
    }

    [HttpDelete("{id}")]
    public Task Delete(long id)
    {
        return service.Delete(id);
    }
}