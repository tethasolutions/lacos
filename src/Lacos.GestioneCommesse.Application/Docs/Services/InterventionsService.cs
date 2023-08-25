using AutoMapper;
using Lacos.GestioneCommesse.Application.Docs.DTOs;
using Lacos.GestioneCommesse.Dal;
using Lacos.GestioneCommesse.Domain.Docs;
using Lacos.GestioneCommesse.Domain.Registry;
using Lacos.GestioneCommesse.Framework.Exceptions;
using Lacos.GestioneCommesse.Framework.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Lacos.GestioneCommesse.Application.Docs.Services;

public class InterventionsService : IInterventionsService
{
    private readonly IMapper mapper;
    private readonly IRepository<Intervention> repository;
    private readonly IRepository<Operator> operatorRepository;
    private readonly ILacosDbContext dbContext;

    public InterventionsService(
        IMapper mapper,
        IRepository<Intervention> repository,
        IRepository<Operator> operatorRepository,
        ILacosDbContext dbContext
    )
    {
        this.mapper = mapper;
        this.repository = repository;
        this.operatorRepository = operatorRepository;
        this.dbContext = dbContext;
    }

    public IQueryable<InterventionReadModel> Query()
    {
        return repository.Query()
            .Project<InterventionReadModel>(mapper);
    }

    public async Task<InterventionDto> Get(long id)
    {
        var interventionDto = await repository.Query()
            .Where(e => e.Id == id)
            .Project<InterventionDto>(mapper)
            .FirstOrDefaultAsync();

        if (interventionDto == null)
        {
            throw new NotFoundException($"Intervento con Id {id} non trovato.");
        }

        return interventionDto;
    }

    public async Task<InterventionDto> Create(InterventionDto interventionDto)
    {
        var intervention = interventionDto.MapTo<Intervention>(mapper);
        
        intervention.Operators.AddRange(
            await operatorRepository.Query()
                .Where(e => interventionDto.Operators.Contains(e.Id))
                .ToListAsync()
        );

        await repository.Insert(intervention);

        await dbContext.SaveChanges();

        return await Get(intervention.Id);
    }

    public async Task<InterventionDto> Update(InterventionDto interventionDto)
    {
        var intervention = await repository.Query()
            .Include(e => e.Operators)
            .Include(e => e.Products)
            .ThenInclude(e => e.CheckList)
            .ThenInclude(e => e!.Items)
            .Where(e => e.Id == interventionDto.Id)
            .FirstOrDefaultAsync();

        if (intervention == null)
        {
            throw new NotFoundException($"Intervento con Id {interventionDto.Id} non trovato.");
        }

        if (intervention.Status != InterventionStatus.Scheduled)
        {
            throw new LacosException("Non puoi modificare un intervento già completato.");
        }

        intervention = interventionDto.MapTo(intervention, mapper);

        intervention.Operators.Clear();
        intervention.Operators.AddRange(
            await operatorRepository.Query()
                .Where(e => interventionDto.Operators.Contains(e.Id))
                .ToListAsync()
        );

        repository.Update(intervention);

        await dbContext.SaveChanges();

        return await Get(intervention.Id);
    }

    public async Task Delete(long id)
    {
        var intervention = await repository.Query()
            .AsSplitQuery()
            .Include(e => e.Products)
            .ThenInclude(e => e.CheckList)
            .ThenInclude(e => e!.Items)
            .FirstOrDefaultAsync(e => e.Id == id);

        if (intervention == null)
        {
            return;
        }

        if (intervention.IsCompleted())
        {
            throw new LacosException("Non puoi eliminare un intervento già completato.");
        }

        repository.Delete(intervention);

        await dbContext.SaveChanges();
    }
}