using Lacos.GestioneCommesse.Application.Docs.DTOs;

namespace Lacos.GestioneCommesse.Application.Docs.Services;

public interface IActivitiesService
{
    IQueryable<ActivityReadModel> Query();
    IQueryable<ActivityReadModel> GetActivitiesFromProduct(string product);
    Task<ActivityDto> Get(long id);
    Task<ActivityDetailDto> GetDetail(long id);
    Task<ActivityDto> Create(ActivityDto activityDto);
    Task<ActivityDto> Update(ActivityDto activityDto);
    Task Delete(long id);
    Task AssignAllCustomerProducts(long id);
    Task AssignAllCustomerProductsMonthlyMaint(long id);
    Task<IEnumerable<ActivityCounterDto>> GetActivitiesCounters();
    Task<ActivityCounterNewDto> GetNewActivitiesCounter();

    Task<IEnumerable<ActivityAttachmentReadModel>> GetActivityAttachments(long jobId, long activityId);
    Task<ActivityAttachmentReadModel> GetActivityAttachmentDetail(long attachmentId);
    Task<ActivityAttachmentDto> UpdateActivityAttachment(long id, ActivityAttachmentDto attachmentDto);
    Task<ActivityAttachmentDto> CreateActivityAttachment(ActivityAttachmentDto attachmentDto);
    Task<ActivityAttachmentReadModel> DownloadActivityAttachment(string filename);
}