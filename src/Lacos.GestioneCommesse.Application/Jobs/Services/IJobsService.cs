using Lacos.GestioneCommesse.Application.Jobs.DTOs;

namespace Lacos.GestioneCommesse.Application.Jobs.Services;

public interface IJobsService
{
    IQueryable<JobReadModel> Query();
    Task<JobDto> Get(long id);
    Task<JobDto> Create(JobDto jobDto);
    Task<JobDto> Update(JobDto jobDto);
    Task Delete(long id);
}