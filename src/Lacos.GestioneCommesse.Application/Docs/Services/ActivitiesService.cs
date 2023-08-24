using AutoMapper;
using Lacos.GestioneCommesse.Application.Docs.DTOs;
using Lacos.GestioneCommesse.Dal;
using Lacos.GestioneCommesse.Domain.Docs;
using Lacos.GestioneCommesse.Framework.Exceptions;
using Lacos.GestioneCommesse.Framework.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Lacos.GestioneCommesse.Application.Docs.Services;

public class ActivitiesService : IActivitiesService
{
    private readonly IMapper mapper;
    private readonly IRepository<Activity> repository;
    private readonly ILacosDbContext dbContext;

    public ActivitiesService(
        IMapper mapper,
        IRepository<Activity> repository,
        ILacosDbContext dbContext
    )
    {
        this.mapper = mapper;
        this.repository = repository;
        this.dbContext = dbContext;
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
            .Include(e => e.Products)
            .ThenInclude(e => e.CheckList)
            .ThenInclude(e => e!.Items)
            .FirstOrDefaultAsync(e => e.Id == id);

        if (activity == null)
        {
            return;
        }

        switch (activity.Status)
        {
            case ActivityStatus.Pending:
                if (activity.HasInterventions())
                {
                    activity.Cancel();
                    repository.Update(activity);
                }
                else
                {
                    repository.Delete(activity);
                }

                break;
            case ActivityStatus.Canceled:
                throw new LacosException("L'attività è già stata annullata.");
            case ActivityStatus.InProgress:
            case ActivityStatus.Completed:
                throw new LacosException("Impossibile eliminare un'attività in corso o completata.");
            default:
                throw new ArgumentOutOfRangeException();
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