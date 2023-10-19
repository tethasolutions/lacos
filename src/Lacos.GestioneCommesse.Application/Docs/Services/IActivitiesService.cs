using Lacos.GestioneCommesse.Application.Docs.DTOs;

namespace Lacos.GestioneCommesse.Application.Docs.Services;

public interface IActivitiesService
{
    IQueryable<ActivityReadModel> Query();
    Task<ActivityDto> Get(long id);
    Task<ActivityDetailDto> GetDetail(long id);
    Task<ActivityDto> Create(ActivityDto activityDto);
    Task<ActivityDto> Update(ActivityDto activityDto);
    Task Delete(long id);
    Task AssignAllCustomerProducts(long id);
}