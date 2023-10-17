using AutoMapper;
using Lacos.GestioneCommesse.Application.Docs.DTOs;
using Lacos.GestioneCommesse.Dal;
using Lacos.GestioneCommesse.Domain.Docs;
using Lacos.GestioneCommesse.Domain.Registry;
using Lacos.GestioneCommesse.Framework.Exceptions;
using Lacos.GestioneCommesse.Framework.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Lacos.GestioneCommesse.Application.Docs.Services;

public class ActivitiesService : IActivitiesService
{
    private readonly IMapper mapper;
    private readonly IRepository<Activity> repository;
    private readonly ILacosDbContext dbContext;
    private readonly IRepository<ActivityProduct> activityProductRepository;
    private readonly IRepository<Product> productRepository;

    public ActivitiesService(
        IMapper mapper,
        IRepository<Activity> repository,
        ILacosDbContext dbContext,
        IRepository<ActivityProduct> activityProductRepository,
        IRepository<Product> productRepository
    )
    {
        this.mapper = mapper;
        this.repository = repository;
        this.dbContext = dbContext;
        this.activityProductRepository = activityProductRepository;
        this.productRepository = productRepository;
    }


    public IQueryable<ActivityReadModel> Query()
    {
        return repository.Query()
            .Project<ActivityReadModel>(mapper);
    }

    public async Task<ActivityDto> Get(long id)
    {
        var activityDto = await repository.Query()
            .Where(e => e.Id == id)
            .Project<ActivityDto>(mapper)
            .FirstOrDefaultAsync();

        if (activityDto == null)
        {
            throw new NotFoundException($"Attività con Id {id} non trovata.");
        }

        return activityDto;
    }

    public async Task<ActivityDetailDto> GetDetail(long id)
    {
        var activityDto = await repository.Query()
            .Where(e => e.Id == id)
            .Project<ActivityDetailDto>(mapper)
            .FirstOrDefaultAsync();

        if (activityDto == null)
        {
            throw new NotFoundException($"Attività con Id {id} non trovata.");
        }

        return activityDto;
    }

    public async Task<ActivityDto> Create(ActivityDto activityDto)
    {
        var activity = activityDto.MapTo<Activity>(mapper);
        var number = await GetNextNumber(activityDto.JobId);

        activity.SetNumber(number);

        await repository.Insert(activity);

        await dbContext.SaveChanges();

        return await Get(activity.Id);
    }

    public async Task<ActivityDto> Update(ActivityDto activityDto)
    {
        var activity = await repository.Get(activityDto.Id);

        if (activity == null)
        {
            throw new NotFoundException($"Attività con Id {activityDto.Id} non trovata.");
        }

        activity = activityDto.MapTo(activity, mapper);

        repository.Update(activity);

        await dbContext.SaveChanges();

        return await Get(activity.Id);
    }

    public async Task Delete(long id)
    {
        var activity = await repository.Query()
            .AsSplitQuery()
            .Include(e => e.Interventions)
            .Include(e => e.ActivityProducts)
            .ThenInclude(e => e.InterventionProducts)
            .ThenInclude(e => e.CheckList)
            .ThenInclude(e => e!.Items)
            .FirstOrDefaultAsync(e => e.Id == id);

        if (activity == null)
        {
            return;
        }

        if (activity.HasCompletedInterventions())
        {
            throw new LacosException("Non puoi eliminare un'attività con interventi completati.");
        }

        repository.Delete(activity);

        await dbContext.SaveChanges();
    }

    public async Task AssignAllCustomerProducts(long id)
    {
        var activity = await repository.Query()
            .AsNoTracking()
            .Where(e => e.Id == id)
            .FirstAsync();
        var activityProducts = activityProductRepository.Query()
            .Where(e => e.ActivityId == id);
        var products = productRepository.Query()
            .Where(e => e.CustomerAddressId == activity.CustomerAddressId);
        var missingProducts = await (
                from product in products
                join activityProduct in activityProducts
                    on product.Id equals activityProduct.ProductId
                    into join1
                from activityProduct in join1.DefaultIfEmpty()
                where activityProduct == null
                select product.Id
            )
            .ToListAsync();

        foreach (var missingProduct in missingProducts)
        {
            var missingActivityProduct = new ActivityProduct
            {
                ProductId = missingProduct,
                ActivityId = id
            };

            await activityProductRepository.Insert(missingActivityProduct);
        }

        await dbContext.SaveChanges();
    }

    private async Task<int> GetNextNumber(long jobId)
    {
        var maxNumber = await repository.Query()
            .Where(e => e.JobId == jobId)
            .Select(e => (int?)e.RowNumber)
            .MaxAsync();

        return (maxNumber ?? 0) + 1;
    }
}