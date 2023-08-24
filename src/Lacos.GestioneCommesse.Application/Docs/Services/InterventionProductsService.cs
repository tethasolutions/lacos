using AutoMapper;
using Lacos.GestioneCommesse.Application.Docs.DTOs;
using Lacos.GestioneCommesse.Dal;
using Lacos.GestioneCommesse.Domain.Docs;
using Lacos.GestioneCommesse.Framework.Exceptions;
using Lacos.GestioneCommesse.Framework.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Lacos.GestioneCommesse.Application.Docs.Services;

public class InterventionProductsService : IInterventionProductsService
{
    private readonly IMapper mapper;
    private readonly IRepository<InterventionProduct> repository;
    private readonly ILacosDbContext dbContext;

    public InterventionProductsService(
        IMapper mapper,
        IRepository<InterventionProduct> repository,
        ILacosDbContext dbContext
    )
    {
        this.mapper = mapper;
        this.repository = repository;
        this.dbContext = dbContext;
    }

    public IQueryable<InterventionProductReadModel> Query()
    {
        return repository.Query()
            .Project<InterventionProductReadModel>(mapper);
    }

    public async Task Create(InterventionProductDto interventionProductDto)
    {
        var interventionProduct = interventionProductDto.MapTo<InterventionProduct>(mapper);

        await repository.Insert(interventionProduct);

        await dbContext.SaveChanges();
    }

    public async Task Delete(long id)
    {
        var interventionProduct = await repository.Query()
            .AsSplitQuery()
            .Include(e => e.Intervention)
            .Include(e => e.CheckList)
            .ThenInclude(e => e!.Items)
            .FirstOrDefaultAsync(e => e.Id == id);

        if (interventionProduct == null)
        {
            return;
        }

        switch (interventionProduct.Intervention?.Status)
        {
            case null:
            case InterventionStatus.Pending:
                repository.Delete(interventionProduct);
                await dbContext.SaveChanges();
                break;
            case InterventionStatus.InProgress:
                throw new LacosException("Non puoi rimuovere un prodotto con un intervento in corso.");
            case InterventionStatus.Completed:
                throw new LacosException("Non puoi rimuovere un prodotto con un intervento completato.");
            case InterventionStatus.Canceled:
                throw new LacosException("Non puoi rimuovere un prodotto con un intervento annullato.");
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}