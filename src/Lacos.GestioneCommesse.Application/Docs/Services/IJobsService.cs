﻿using Lacos.GestioneCommesse.Application.Docs.DTOs;

namespace Lacos.GestioneCommesse.Application.Docs.Services;

public interface IJobsService
{
    IQueryable<JobReadModel> Query();
    Task<JobDto> Get(long id);
    Task<JobDto> Create(JobDto jobDto);
    Task<JobDto> Update(JobDto jobDto);
    Task Delete(long id);
}