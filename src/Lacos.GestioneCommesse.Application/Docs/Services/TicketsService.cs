using AutoMapper;
using Lacos.GestioneCommesse.Application.Docs.DTOs;
using Lacos.GestioneCommesse.Dal;
using Lacos.GestioneCommesse.Domain.Docs;
using Lacos.GestioneCommesse.Framework.Exceptions;
using Lacos.GestioneCommesse.Framework.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Lacos.GestioneCommesse.Application.Docs.Services;

public class TicketsService : ITicketsService
{
    private readonly IMapper mapper;
    private readonly IRepository<Ticket> repository;
    private readonly ILacosDbContext dbContext;

    public TicketsService(
        IMapper mapper,
        IRepository<Ticket> repository,
        ILacosDbContext dbContext
    )
    {
        this.mapper = mapper;
        this.repository = repository;
        this.dbContext = dbContext;
    }

    public IQueryable<TicketReadModel> Query()
    {
        return repository.Query()
            .Project<TicketReadModel>(mapper);
    }

    public async Task<TicketDto> Get(long id)
    {
        var TicketDto = await repository.Query()
            .Where(e => e.Id == id)
            .Project<TicketDto>(mapper)
            .FirstOrDefaultAsync();

        if (TicketDto == null)
        {
            throw new NotFoundException($"Commessa con Id {id} non trovata.");
        }

        return TicketDto;
    }

    public async Task<TicketDto> Create(TicketDto TicketDto)
    {
        var Ticket = TicketDto.MapTo<Ticket>(mapper);
        var number = await GetNextNumber(Ticket.TicketDate.Year);

        Ticket.SetCode(Ticket.TicketDate.Year, number);
        Ticket.IsNew= true;

        await repository.Insert(Ticket);

        await dbContext.SaveChanges();

        return await Get(Ticket.Id);
    }

    public async Task<TicketDto> Update(TicketDto TicketDto)
    {
        var Ticket = await repository.Get(TicketDto.Id);

        if (Ticket == null)
        {
            throw new NotFoundException($"Commessa con Id {TicketDto.Id} non trovata.");
        }

        Ticket = TicketDto.MapTo(Ticket, mapper);

        Ticket.IsNew = false;

        repository.Update(Ticket);

        await dbContext.SaveChanges();

        return await Get(Ticket.Id);
    }

    public async Task Delete(long id)
    {
        var Ticket = await repository.Query()
            .AsSplitQuery()
            .FirstOrDefaultAsync(e => e.Id == id);

        if (Ticket == null)
        {
            return;
        }

        repository.Delete(Ticket);
        
        await dbContext.SaveChanges();
    }

    public async Task<int> GetNextNumber(int year)
    {
        var maxNumber = await repository.Query()
            .Where(e => e.Year == year)
            .Select(e => (int?)e.Number)
            .MaxAsync();

        return (maxNumber ?? 0) + 1;
    }

    public async Task<TicketCounterDto> GetTicketsCounters()
    {
        var query = repository.Query()
            .AsNoTracking()
            .Where(e => e.Status == TicketStatus.Opened)
            .GroupBy(e => e.Status)
            .Select(group => new TicketCounterDto
            {
                OpenedTickets = group.Where(e => !e.IsNew).Count(),
                NewTickets = group.Where(e => e.IsNew).Count()
            })
            //.Project<ActivityCounterDto>(mapper)
            .FirstOrDefaultAsync();

        return await query;

    }
}