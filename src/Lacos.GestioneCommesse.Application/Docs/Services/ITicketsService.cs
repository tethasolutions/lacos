using Lacos.GestioneCommesse.Application.Docs.DTOs;

namespace Lacos.GestioneCommesse.Application.Docs.Services;

public interface ITicketsService
{
    IQueryable<TicketReadModel> Query();
    Task<TicketDto> Get(long id);
    Task<TicketDto> Create(TicketDto jobDto);
    Task<TicketDto> Update(TicketDto jobDto);
    Task Delete(long id);
}