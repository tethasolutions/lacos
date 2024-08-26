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
    private readonly IRepository<ActivityType> activityTypeRepository;
    private readonly IRepository<Product> productRepository;
    private readonly IRepository<Job> jobRepository;
    private readonly IRepository<Ticket> ticketRepository;
    private readonly ILacosSession session;

    private static readonly Expression<Func<Job, JobStatus>> StatusExpression = j =>
    j.Activities
    .All(e => e.Type.InfluenceJobStatus != true) 
    ? j.Status
    :
        j.Activities.Where(e => e.Type.InfluenceJobStatus == true).All(a => a.Status == ActivityStatus.Completed)
        ?
            JobStatus.Completed
        : j.Activities
                .Any(a => a.Status == ActivityStatus.InProgress && a.Type.InfluenceJobStatus.GetValueOrDefault())
                ? JobStatus.InProgress
                : j.Activities
                    .Any(a => a.Status == ActivityStatus.Pending && a.Type.InfluenceJobStatus.GetValueOrDefault())
                    ? JobStatus.Pending
                    : j.Status;

    public ActivitiesService(
        IMapper mapper,
        IRepository<Activity> repository,
        ILacosDbContext dbContext,
        IRepository<ActivityProduct> activityProductRepository,
        IRepository<Product> productRepository,
        IRepository<Job> jobRepository,
        IRepository<Ticket> ticketRepository,
        ILacosSession session, 
        IRepository<ActivityAttachment> activityAttachmentRepository,
        IRepository<ActivityType> activityTypeRepository
    )
    {
        this.mapper = mapper;
        this.repository = repository;
        this.dbContext = dbContext;
        this.activityProductRepository = activityProductRepository;
        this.activityAttachmentRepository= activityAttachmentRepository;
        this.activityTypeRepository = activityTypeRepository;
        this.productRepository = productRepository;
        this.jobRepository = jobRepository;
        this.ticketRepository = ticketRepository;
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

        //activityDto.Messages = activityDto.Messages.Where(m => m.OperatorId == session.CurrentUser.OperatorId || 
        //    m.TargetOperatorsId.Contains(session.CurrentUser.OperatorId.ToString()));

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

        ActivityType activityType = await activityTypeRepository.Get(activity.TypeId);
        if ((bool)activityType.InfluenceJobStatus && activity.JobId != null)
        {
            Job job = await jobRepository.Query()
                .Where(e => e.Id == activity.JobId)
                .Include(e => e.Activities)
                .FirstOrDefaultAsync();
            if (job != null)
            {
                if (job.Status != JobStatus.Billing && job.Status != JobStatus.Billed)
                {
                    Func<Job, JobStatus> statusDelegate = StatusExpression.Compile();
                    job.Status = statusDelegate(job);
                    jobRepository.Update(job);
                    await dbContext.SaveChanges();
                }
            }
        }

        return await Get(activity.Id);
    }

    public async Task<ActivityDto> Update(ActivityDto activityDto)
    {
        var activity = await repository
            .Query()
            .Where(x => x.Id == activityDto.Id)
            .Include(x => x.Attachments)
            .Include(x => x.Messages)
            //.Include(x => x.Messages.Where(m => m.OperatorId == session.CurrentUser.OperatorId || 
            //    m.MessageNotifications.Any(mn => mn.OperatorId == session.CurrentUser.OperatorId)))
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

        ActivityType activityType = await activityTypeRepository.Get(activity.TypeId);
        if ((bool)activityType.InfluenceJobStatus && activity.JobId != null)
        {
            Job job = await jobRepository.Query()
                .Where(e => e.Id == activity.JobId)
                .Include(e => e.Activities)
                .ThenInclude(e => e.Type)
                .FirstOrDefaultAsync();
            if (job != null)
            {
                if (job.Status != JobStatus.Billing && job.Status != JobStatus.Billed)
                {
                    Func<Job, JobStatus> statusDelegate = StatusExpression.Compile();
                    job.Status = statusDelegate(job);
                    jobRepository.Update(job);
                    await dbContext.SaveChanges();
                }
            }
        }

        Ticket ticket = await ticketRepository.Query()
            .Where(x => x.ActivityId == activity.Id)
            .FirstOrDefaultAsync();
        if (ticket != null)
        {
            ticket.Status = TicketStatus.Resolved;
            ticketRepository.Update(ticket);
            await dbContext.SaveChanges();
        }

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
    public async Task<IEnumerable<ActivityAttachmentReadModel>> GetActivityAttachments(long jobId, long activityId)
    {
        if (activityId != 0)
        {
            var activityAdminAttachments = await activityAttachmentRepository
                .Query()
                .AsNoTracking()
                .Where(x => x.Activity.Id == activityId)
                .OrderBy(x => x.CreatedOn)
                .ToArrayAsync();

            return activityAdminAttachments.MapTo<IEnumerable<ActivityAttachmentReadModel>>(mapper);
        }

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

    public IQueryable<ActivityReadModel> GetActivitiesFromProduct(string product)
    {
        var query = repository.Query()
            .Where(e => e.ActivityProducts.Any(p => p.Product.Code == product || (p.Product.QrCodePrefix + p.Product.QrCodeNumber) == product));

        return query
            .AsQueryable()
            .Project<ActivityReadModel>(mapper);

    }
}