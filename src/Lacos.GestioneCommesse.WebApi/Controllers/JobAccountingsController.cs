using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Lacos.GestioneCommesse.Application.Docs.DTOs;
using Lacos.GestioneCommesse.Application.Docs.Services;
using Lacos.GestioneCommesse.WebApi.ModelBinders;
using Microsoft.AspNetCore.Mvc;

namespace Lacos.GestioneCommesse.WebApi.Controllers;

[Route("api/jobaccountings")]
public class JobAccountingsController : LacosApiController
{
    private readonly IJobAccountingService service;

    public JobAccountingsController(IJobAccountingService service)
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
    public Task Create(JobAccountingDto jobAccountingDto)
    {
        return service.Create(jobAccountingDto);
    }

    [HttpGet("{id}")]
    public Task<JobAccountingDto> Get(long id)
    {
        return service.Get(id);
    }

    [HttpPut("{id}")]
    public Task<JobAccountingDto> Update(long id, JobAccountingDto jobAccountingDto)
    {
        jobAccountingDto.Id = id;

        return service.Update(jobAccountingDto);
    }

    [HttpDelete("{id}")]
    public Task Delete(long id)
    {
        return service.Delete(id);
    }
}