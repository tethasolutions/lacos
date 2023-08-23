using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Lacos.GestioneCommesse.Application.Jobs.DTOs;
using Lacos.GestioneCommesse.Application.Jobs.Services;
using Microsoft.AspNetCore.Mvc;

namespace Lacos.GestioneCommesse.WebApi.Controllers;

public class JobsController : LacosApiController
{
    private readonly IJobsService service;

    public JobsController(IJobsService service)
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
    public Task<JobDto> Get(long id)
    {
        return service.Get(id);
    }

    [HttpPost]
    public Task<JobDto> Create(JobDto jobDto)
    {
        return service.Create(jobDto);
    }

    [HttpPut("{id}")]
    public Task<JobDto> Update(long id, JobDto jobDto)
    {
        jobDto.Id = id;

        return service.Update(jobDto);
    }

    [HttpDelete("{id}")]
    public Task Delete(long id)
    {
        return service.Delete(id);
    }
}