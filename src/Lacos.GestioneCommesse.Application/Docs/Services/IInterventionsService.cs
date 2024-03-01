using Lacos.GestioneCommesse.Application.Docs.DTOs;

namespace Lacos.GestioneCommesse.Application.Docs.Services;

public interface IInterventionsService
{
    IQueryable<InterventionReadModel> Query();
    Task<InterventionDto> Get(long id);
    Task<InterventionDto> Create(InterventionDto interventionDto);
    Task<InterventionDto> Update(InterventionDto interventionDto);
    Task Delete(long id);

    IQueryable<InterventionProductReadModel> GetProductsByIntervention(long id);
    Task<InterventionProductCheckListDto> GetInterventionProductCheckList(long interventionProductId);
    
    Task<ReportDto> GenerateReport(long interventionId);
    Task<InterventionNoteDto> DownloadInterventionNote(string filename);
}