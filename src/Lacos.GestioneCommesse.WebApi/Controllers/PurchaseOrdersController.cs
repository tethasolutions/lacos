using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Lacos.GestioneCommesse.Application.Docs.DTOs;
using Lacos.GestioneCommesse.Application.Docs.Services;
using Microsoft.AspNetCore.Mvc;

namespace Lacos.GestioneCommesse.WebApi.Controllers;

[Route("api/purchase-orders")]
public class PurchaseOrdersController : LacosApiController
{
    private readonly IPurchaseOrdersService service;

    public PurchaseOrdersController(IPurchaseOrdersService service)
    {
        this.service = service;
    }

    [HttpGet("read")]
    public Task<DataSourceResult> Read(DataSourceRequest request)
    {
        return service.Query()
            .ToDataSourceResultAsync(request);
    }

    [HttpGet("{id}")]
    public Task<PurchaseOrderDto> Get(long id)
    {
        return service.Get(id);
    }

    [HttpPost]
    public Task<PurchaseOrderDto> Create(PurchaseOrderDto purchaseOrderDto)
    {
        return service.Create(purchaseOrderDto);
    }

    [HttpPut("{id}")]
    public Task<PurchaseOrderDto> Update(long id, PurchaseOrderDto purchaseOrderDto)
    {
        purchaseOrderDto.Id = id;

        return service.Update(purchaseOrderDto);
    }

    [HttpDelete("{id}")]
    public Task Delete(long id)
    {
        return service.Delete(id);
    }

}