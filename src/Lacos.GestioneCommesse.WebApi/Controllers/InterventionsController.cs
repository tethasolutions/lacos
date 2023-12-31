﻿using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Lacos.GestioneCommesse.Application.Docs.DTOs;
using Lacos.GestioneCommesse.Application.Docs.Services;
using Lacos.GestioneCommesse.Application.Products.Service;
using Lacos.GestioneCommesse.WebApi.Auth;
using Microsoft.AspNetCore.Mvc;

namespace Lacos.GestioneCommesse.WebApi.Controllers;

[RequireUser]
public class InterventionsController : LacosApiController
{
    private readonly IInterventionsService service;

    public InterventionsController(IInterventionsService service)
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
    public Task<InterventionDto> Get(long id)
    {
        return service.Get(id);
    }

    [HttpPost]
    public Task<InterventionDto> Create(InterventionDto interventionDto)
    {
        return service.Create(interventionDto);
    }

    [HttpPut("{id}")]
    public Task<InterventionDto> Update(long id, InterventionDto interventionDto)
    {
        interventionDto.Id = id;

        return service.Update(interventionDto);
    }

    [HttpDelete("{id}")]
    public Task Delete(long id)
    {
        return service.Delete(id);
    }

    [HttpGet("intervention-checklist-by-product/{id}")]
    public Task<InterventionProductCheckListDto> GetCheckListItemsByProduct(long id)
    {
        return service.GetInterventionProductCheckList(id);
    }

    [HttpGet("intervention-products-by-intervention/{id}")]
    public async Task<DataSourceResult> GetInterventionProductsByIntervention([DataSourceRequest] DataSourceRequest request, long id)
    {
        var products = (service.GetProductsByIntervention(id));
        return await products.ToDataSourceResultAsync(request);
    }

    [HttpGet("download-report/{interventionId}")]
    public async Task<FileResult> DownloadReport(long interventionId)
    {
        var report = await service.GenerateReport(interventionId);
        return File(report.Content, "application/pdf", report.FileName);
    }
}