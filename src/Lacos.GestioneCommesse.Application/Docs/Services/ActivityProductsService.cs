﻿using AutoMapper;
using Lacos.GestioneCommesse.Application.Docs.DTOs;
using Lacos.GestioneCommesse.Dal;
using Lacos.GestioneCommesse.Domain.Docs;
using Lacos.GestioneCommesse.Framework.Exceptions;
using Lacos.GestioneCommesse.Framework.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Lacos.GestioneCommesse.Application.Docs.Services;

public class ActivityProductsService : IActivityProductsService
{
    private readonly IMapper mapper;
    private readonly IRepository<ActivityProduct> repository;
    private readonly ILacosDbContext dbContext;

    public ActivityProductsService(
        IMapper mapper,
        IRepository<ActivityProduct> repository,
        ILacosDbContext dbContext
    )
    {
        this.mapper = mapper;
        this.repository = repository;
        this.dbContext = dbContext;
    }

    public IQueryable<ActivityProductReadModel> Query()
    {
        return repository.Query()
            .Project<ActivityProductReadModel>(mapper);
    }

    public async Task Create(ActivityProductDto activityProductDto)
    {
        var activityProduct = activityProductDto.MapTo<ActivityProduct>(mapper);

        await repository.Insert(activityProduct);

        await dbContext.SaveChanges();
    }

    public async Task Delete(long id)
    {
        var activityProduct = await repository.Query()
            .AsSplitQuery()
            .Include(e => e.InterventionProducts)
            .ThenInclude(e => e.CheckList)
            .ThenInclude(e => e!.Items)
            .Include(e => e.InterventionProducts)
            .ThenInclude(e => e.Intervention)
            .FirstOrDefaultAsync(e => e.Id == id);

        if (activityProduct == null)
        {
            return;
        }

        if (activityProduct.HasCompletedInterventions())
        {
            throw new LacosException("Non puoi rimuovere un prodotto con un intervento completato.");
        }

        repository.Delete(activityProduct);

        await dbContext.SaveChanges();
    }

    public async Task Duplicate(long id)
    {
        var activityProduct = await repository.Get(id);

        if (activityProduct == null)
        {
            throw new NotFoundException($"Prodotto con Id {id} non trovato.");
        }

        activityProduct = new ActivityProduct
        {
            ActivityId = activityProduct.ActivityId,
            ProductId = activityProduct.ProductId
        };

        await repository.Insert(activityProduct);

        await dbContext.SaveChanges();
    }
}