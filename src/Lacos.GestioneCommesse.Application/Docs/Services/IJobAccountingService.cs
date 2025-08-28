using Lacos.GestioneCommesse.Application.Docs.DTOs;

namespace Lacos.GestioneCommesse.Application.Docs.Services;

public interface IJobAccountingService
{
    public IQueryable<JobAccountingReadModel> Query();
    Task Create(JobAccountingDto jobAccountingDto);
    Task<JobAccountingDto> Get(long id);
    Task<JobAccountingDto> Update(JobAccountingDto jobAccountingDto);
    Task Delete(long id);
}