using Lacos.GestioneCommesse.Application.Docs.DTOs;
using System.Net.Mail;

namespace Lacos.GestioneCommesse.Application.Docs.Services;

public interface IInterventionsService
{
    IQueryable<InterventionReadModel> Query(bool filterHistorical);
    IQueryable<InterventionSingleProductReadModel> QuerySingleProduct(long activityId, string product);
    Task<InterventionDto> Get(long id);
    Task<InterventionDto> Create(InterventionDto interventionDto);
    Task<InterventionDto> Update(InterventionDto interventionDto);

    Task UpdateActivityStatus(long id);

    Task Delete(long id);

    IQueryable<InterventionProductReadModel> GetProductsByIntervention(long id);
    Task<InterventionProductCheckListDto> GetInterventionProductCheckList(long interventionProductId);
    Task<IEnumerable<InterventionNoteDto>> GetInterventionAttachments(long jobId, long activityId);

    Task<ReportDto> GenerateReport(long interventionId);
    Task<InterventionNoteDto> DownloadInterventionNote(string filename);
    IQueryable<InterventionProductCheckListItemKOReadModel> GetInterventionsKo();

}