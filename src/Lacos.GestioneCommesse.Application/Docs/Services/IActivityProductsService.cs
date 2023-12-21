using Lacos.GestioneCommesse.Application.Docs.DTOs;

namespace Lacos.GestioneCommesse.Application.Docs.Services;

public interface IActivityProductsService
{
    public IQueryable<ActivityProductReadModel> Query();
    Task Create(ActivityProductDto activityProductDto);
    Task<ActivityProductDto> Get(long id);
    Task<ActivityProductDto> Update(ActivityProductDto activityProductDto);
    Task Delete(long id);
    Task Duplicate(long id);
}