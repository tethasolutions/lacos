using Lacos.GestioneCommesse.Application.Docs.DTOs;

namespace Lacos.GestioneCommesse.Application.Docs.Services;

public interface IInterventionProductsService
{
    public IQueryable<InterventionProductReadModel> Query();
    Task Create(InterventionProductDto interventionProductDto);
    Task Delete(long id);
}