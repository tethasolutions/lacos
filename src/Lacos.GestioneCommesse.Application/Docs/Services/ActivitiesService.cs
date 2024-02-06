using AutoMapper;
using Lacos.GestioneCommesse.Application.Docs.DTOs;
using Lacos.GestioneCommesse.Application.Registry.DTOs;
using Lacos.GestioneCommesse.Dal;
using Lacos.GestioneCommesse.Domain.Docs;
using Lacos.GestioneCommesse.Domain.Registry;
using Lacos.GestioneCommesse.Domain.Security;
using Lacos.GestioneCommesse.Framework.Exceptions;
using Lacos.GestioneCommesse.Framework.Extensions;
using Lacos.GestioneCommesse.Framework.Session;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace Lacos.GestioneCommesse.Application.Docs.Services;

public class ActivitiesService : IActivitiesService
{
    private readonly IMapper mapper;
    private readonly IRepository<Activity> repository;
    private readonly ILacosDbContext dbContext;
    private readonly IRepository<ActivityProduct> activityProductRepository;
    private readonly IRepository<ActivityAttachment> activityAttachmentRepository;
    private readonly IRepository<Product> productRepository;
    private readonly IRepository<Job> jobRepository;
    private readonly ILacosSession session;

    public ActivitiesService(
        IMapper mapper,
        IRepository<Activity> repository,
        ILacosDbContext dbContext,
        IRepository<ActivityProduct> activityProductRepository,
        IRepository<Product> productRepository,
        IRepository<Job> jobRepository,
        ILacosSession session, 
        IRepository<ActivityAttachment> activityAttachmentRepository
    )
    {
        this.mapper = mapper;
        this.repository = repository;
        this.dbContext = dbContext;
        this.activityProductRepository = activityProductRepository;
        this.activityAttachmentRepository= activityAttachmentRepository;
        this.productRepository = productRepository;
        this.jobRepository = jobRepository;
        this.session = session;
    }


    public IQueryable<ActivityReadModel> Query()
    {
        var query = repository.Query();

        if (session.IsAuthenticated() && session.IsAuthorized(Role.Operator))
        {
            var user = session.CurrentUser!;

            query = query
                .Where(e =>
                    e.Type!.Operators.Any(o => o.Id == user.OperatorId) ||
                    e.Interventions.Any(i => i.Operators.Any(o => o.Id == user.OperatorId))
                );
        }

        return query
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
        activity.IsNewReferent = true;

        await repository.Insert(activity);

        foreach (var file in activity.Attachments)
        {
            var activityAttachment = file.MapTo<ActivityAttachment>(mapper);
            activityAttachment.ActivityId = activity.Id;
            activityAttachment.Activity = activity;
            activityAttachment.DisplayName = file.DisplayName;
            activityAttachment.FileName = file.FileName;

            await activityAttachmentRepository.Insert(activityAttachment);
        }

        await dbContext.SaveChanges();

        return await Get(activity.Id);
    }

    public async Task<ActivityDto> Update(ActivityDto activityDto)
    {
        var activity = await repository
            .Query()
            .Where(x => x.Id == activityDto.Id)
            .Include(x => x.Attachments)
            .SingleOrDefaultAsync();

        if (activity == null)
        {
            throw new NotFoundException($"Attività con Id {activityDto.Id} non trovata.");
        }

        bool IsReferentChanged = activity.ReferentId != activityDto.ReferentId;

        activity = activityDto.MapTo(activity, mapper);

        //reset isNewReferent flag
        activity.IsNewReferent = IsReferentChanged;

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
            .Where(e => e.AddressId == activity.AddressId);
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
    public async Task<IEnumerable<ActivityCounterDto>> GetActivitiesCounters()
    {
        if (!(session.IsAuthenticated())) return null;
        
        var user = session.CurrentUser!;

        if (user.Role == Role.Administrator)
        {
            var query = repository.Query()
                .AsNoTracking()
                .Where(e => e.Status != ActivityStatus.Completed)
                .GroupBy(e => e.TypeId)
                .Select(group => new ActivityCounterDto
                {
                    ActivityId = group.Key,
                    ActivityName = group.First().Type.Name,
                    ActivityColor = group.First().Type.ColorHex,
                    Active = group.Where(e => e.ExpirationDate >= DateTimeOffset.UtcNow.Date).Count(),
                    Expired = group.Where(e => e.ExpirationDate < DateTimeOffset.UtcNow.Date).Count()
                })
                //.Project<ActivityCounterDto>(mapper)
                .OrderBy(e => e.ActivityName)
                .ToListAsync();
            return await query;
        }
        else
        {
            var query = repository.Query()
                .AsNoTracking()
                .Where(e => e.Status != ActivityStatus.Completed && e.Type!.Operators.Any(o => o.Id == user.OperatorId))
                .GroupBy(e => e.TypeId)
                .Select(group => new ActivityCounterDto
                {
                    ActivityId = group.Key,
                    ActivityName = group.First().Type.Name,
                    ActivityColor = group.First().Type.ColorHex,
                    Active = group.Where(e => e.ExpirationDate >= DateTimeOffset.UtcNow.Date).Count(),
                    Expired = group.Where(e => e.ExpirationDate < DateTimeOffset.UtcNow.Date).Count()
                })
                //.Project<ActivityCounterDto>(mapper)
                .OrderBy(e => e.ActivityName)
                .ToListAsync();
            return await query;
        }

    }

    // --------------------------------------------------------------------------------------------------------------
    public async Task<IEnumerable<ActivityAttachmentReadModel>> GetActivityAttachments(long jobId)
    {
        var activityAttachments = await activityAttachmentRepository
            .Query()
            .AsNoTracking()
            .Where(x => x.Activity.JobId == jobId)
            .OrderBy(x => x.CreatedOn)
            .ToArrayAsync();

        return activityAttachments.MapTo<IEnumerable<ActivityAttachmentReadModel>>(mapper);
    }

    public async Task<ActivityAttachmentReadModel> GetActivityAttachmentDetail(long attachmentId)
    {
        var activityAttachment = await activityAttachmentRepository
            .Query()
            .AsNoTracking()
            .Where(x => x.Id == attachmentId)
            .SingleOrDefaultAsync();

        return activityAttachment.MapTo<ActivityAttachmentReadModel>(mapper);
    }

    public async Task<ActivityAttachmentReadModel> DownloadActivityAttachment(string filename)
    {
        var activityAttachment = await activityAttachmentRepository
            .Query()
            .AsNoTracking()
            .Where(x => x.FileName == filename)
            .SingleOrDefaultAsync();

        return activityAttachment.MapTo<ActivityAttachmentReadModel>(mapper);
    }

    public async Task<ActivityAttachmentDto> UpdateActivityAttachment(long id, ActivityAttachmentDto attachmentDto)
    {
        var attachment = await activityAttachmentRepository.Get(id);

        if (attachment == null)
        {
            throw new NotFoundException("Errore allegato");
        }
        attachmentDto.MapTo(attachment, mapper);

        activityAttachmentRepository.Update(attachment);

        await dbContext.SaveChanges();

        return attachment.MapTo<ActivityAttachmentDto>(mapper);
    }

    public async Task<ActivityAttachmentDto> CreateActivityAttachment(ActivityAttachmentDto attachmentDto)
    {
        var attachment = attachmentDto.MapTo<ActivityAttachment>(mapper);

        await activityAttachmentRepository.Insert(attachment);

        await dbContext.SaveChanges();

        return attachment.MapTo<ActivityAttachmentDto>(mapper);
    }
    //------------------------------------------------------------------------------------------------------------

    public async Task<ActivityCounterNewDto> GetNewActivitiesCounter()
    {
        var user = session.CurrentUser!;

        var query = await repository.Query()
            .AsNoTracking()
                .AsNoTracking()
                .Where(e => e.ReferentId == user.OperatorId)
            .GroupBy(e => e.ReferentId)
            .Select(group => new ActivityCounterNewDto
            {
                NewActivities = group.Where(e => e.IsNewReferent).Count()
            })
            .FirstOrDefaultAsync();

        return query ?? new ActivityCounterNewDto();

    }
}