using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Lacos.GestioneCommesse.Application.Docs.DTOs;
using Lacos.GestioneCommesse.Application.Docs.Services;
using Microsoft.AspNetCore.Mvc;

namespace Lacos.GestioneCommesse.WebApi.Controllers;

public class TicketsController : LacosApiController
{
    private readonly ITicketsService service;

    public TicketsController(ITicketsService service)
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
    public Task<TicketDto> Get(long id)
    {
        return service.Get(id);
    }

    [HttpPost]
    public Task<TicketDto> Create(TicketDto ticketDto)
    {
        return service.Create(ticketDto);
    }

    [HttpPut("{id}")]
    public Task<TicketDto> Update(long id, TicketDto ticketDto)
    {
        ticketDto.Id = id;

        return service.Update(ticketDto);
    }

    [HttpDelete("{id}")]
    public Task Delete(long id)
    {
        return service.Delete(id);
    }
}