using AutoMapper;
using Lacos.GestioneCommesse.Application.Docs.DTOs;
using Lacos.GestioneCommesse.Dal;
using Lacos.GestioneCommesse.Domain.Docs;
using Lacos.GestioneCommesse.Framework.Exceptions;
using Lacos.GestioneCommesse.Framework.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Lacos.GestioneCommesse.Application.Docs.Services;

public class PurchaseOrdersService : IPurchaseOrdersService
{
    private readonly IMapper mapper;
    private readonly IRepository<PurchaseOrder> repository;
    private readonly IRepository<PurchaseOrderItem> repositoryItem;
    private readonly ILacosDbContext dbContext;

    public PurchaseOrdersService(
        IMapper mapper,
        IRepository<PurchaseOrder> repository,
        IRepository<PurchaseOrderItem> repositoryItem,
        ILacosDbContext dbContext
    )
    {
        this.mapper = mapper;
        this.repository = repository;
        this.repositoryItem = repositoryItem;
        this.dbContext = dbContext;
    }

    public IQueryable<PurchaseOrderReadModel> Query()
    {
        return repository.Query()
            .Project<PurchaseOrderReadModel>(mapper);
    }

    public async Task<PurchaseOrderDto> Get(long id)
    {
        var purchaseOrderDto = await repository.Query()
            .Where(e => e.Id == id)
            .Project<PurchaseOrderDto>(mapper)
            .FirstOrDefaultAsync();

        if (purchaseOrderDto == null)
        {
            throw new NotFoundException($"Ordine con Id {id} non trovato.");
        }

        return purchaseOrderDto;
    }

    public async Task<PurchaseOrderDto> Create(PurchaseOrderDto purchaseOrderDto)
    {
        var purchaseOrder = purchaseOrderDto.MapTo<PurchaseOrder>(mapper);
        var number = await GetNextNumber(purchaseOrder.Date.Year);

        purchaseOrder.SetCode(purchaseOrder.Date.Year, number);

        await repository.Insert(purchaseOrder);

        await dbContext.SaveChanges();

        return await Get(purchaseOrder.Id);
    }

    public async Task<PurchaseOrderDto> Update(PurchaseOrderDto purchaseOrderDto)
    {
        var purchaseOrder = await repository.Query()
            .Include(e => e.Items)
            .FirstOrDefaultAsync(e => e.Id == purchaseOrderDto.Id);

        if (purchaseOrder == null)
        {
            throw new NotFoundException($"Ordine con Id {purchaseOrderDto.Id} non trovato.");
        }

        purchaseOrder = purchaseOrderDto.MapTo(purchaseOrder, mapper);

        repository.Update(purchaseOrder);

        await dbContext.SaveChanges();

        return await Get(purchaseOrder.Id);
    }

    public async Task Delete(long id)
    {
        var purchaseOrder = await repository.Query()
            .Include(e => e.Items)
            .FirstOrDefaultAsync(e => e.Id == id);

        if (purchaseOrder == null)
        {
            return;
        }

        repository.Delete(purchaseOrder);

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
}