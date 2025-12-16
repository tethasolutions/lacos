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
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Linq.Expressions;
using Telerik.Reporting.Interfaces;

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
    private readonly IRepository<PurchaseOrder> purchaseOrderRepository;
    private readonly IRepository<PurchaseOrderExpense> purchaseOrderExpenseRepository;
    private readonly IRepository<GlobalSetting> globalSettingRepository;
    private readonly IPurchaseOrdersService purchaseOrdersService;
    private readonly ILacosSession session;
    private readonly ILogger<ActivitiesService> logger;

    private static readonly Expression<Func<Job, JobStatus>> StatusExpression = j =>
    j.Activities
    .All(e => e.Type.InfluenceJobStatus != true) 
    ? j.Status
    :
        j.Activities.Where(e => e.Type.InfluenceJobStatus == true).All(a => a.Status == ActivityStatus.Completed)
        ?
            JobStatus.Completed
        : j.Activities.Where(e => e.Type.InfluenceJobStatus == true)
                .Any(a => a.Status == ActivityStatus.InProgress || a.Status == ActivityStatus.Ready || a.Status == ActivityStatus.Completed)
                ? JobStatus.InProgress
                : j.Activities.Where(e => e.Type.InfluenceJobStatus == true)
                    .Any(a => a.Status == ActivityStatus.Pending)
                    ? JobStatus.Pending
                    : j.Status;

    public ActivitiesService(
        IMapper mapper,
        IRepository<Activity> repository,
        ILacosDbContext dbContext,
        IRepository<ActivityProduct> activityProductRepository,
        IRepository<ActivityAttachment> activityAttachmentRepository,
        IRepository<ActivityType> activityTypeRepository,
        IRepository<Product> productRepository,
        IRepository<Job> jobRepository,
        IRepository<Ticket> ticketRepository,
        IRepository<PurchaseOrder> purchaseOrderRepository,
        IRepository<PurchaseOrderExpense> purchaseOrderExpenseRepository,
        IRepository<GlobalSetting> globalSettingRepository,
        IPurchaseOrdersService purchaseOrdersService,
        ILacosSession session,
        ILogger<ActivitiesService> logger
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
        this.purchaseOrderRepository = purchaseOrderRepository;
        this.purchaseOrderExpenseRepository = purchaseOrderExpenseRepository;
        this.globalSettingRepository = globalSettingRepository;
        this.purchaseOrdersService = purchaseOrdersService;
        this.session = session;
        this.logger = logger;
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
            var activityDto = await dbContext.ExecuteWithDisabledQueryFilters(async () => await repository.Query()
                .Where(e => e.Id == id)
                .Include(e => e.Type)
                .Project<ActivityDto>(mapper)
                .FirstOrDefaultAsync(), QueryFilter.OperatorEntity);

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
                .ThenInclude(e => e.Type)
                .FirstOrDefaultAsync();
            if (job != null)
            {
                if (job.Status != JobStatus.Billing && job.Status != JobStatus.Billed)
                {
                    try
                    {
                        var PreviousStatus = job.Status;
                        Func<Job, JobStatus> statusDelegate = StatusExpression.Compile();
                        job.Status = statusDelegate(job);
                        logger.LogWarning($"[{activity.JobId}]Commessa {job.Number.ToString("000")}/{job.Year}: " +
                            $"modifica attività {activity.RowNumber}/{activity.Type!.Name}: " +
                            $"cambio stato commessa '{PreviousStatus}' -> '{job.Status}' ");
                        jobRepository.Update(job);
                        await dbContext.SaveChanges();
                    }
                    catch (Exception ex) { }
                }
            }
        }

        await HandleFloorDeliveryAsync(activity);

        return await Get(activity.Id);
    }

    public async Task<ActivityDto> Update(ActivityDto activityDto)
    {
        var activity = await repository
            .Query()
            .Where(x => x.Id == activityDto.Id)
            .Include(x => x.Job)
            .Include(x => x.Type)
            .Include(x => x.Attachments)
            .Include(x => x.Messages)
            .Include(x => x.ActivityDependencies)
            .ThenInclude(y => y.Type)
            .Include(x => x.PurchaseOrderDependencies)
            .Include(x => x.ParentActivities)
            .ThenInclude(x => x.Type)
            .Include(x => x.ParentActivities)
            .ThenInclude(y => y.ActivityDependencies)
            .Include(x => x.ParentActivities)
            .ThenInclude(y => y.PurchaseOrderDependencies)
            .SingleOrDefaultAsync();

        if (activity == null)
        {
            throw new NotFoundException($"Attività con Id {activityDto.Id} non trovata.");
        }

        bool IsReferentChanged = activity.ReferentId != activityDto.ReferentId;
        bool IsFloorDeliveryChanged = activity.IsFloorDelivery != activityDto.IsFloorDelivery;

        activity = activityDto.MapTo(activity, mapper);

        //reset isNewReferent flag
        activity.IsNewReferent = IsReferentChanged;

        //check if has parent activities
        if ((activity.Status == ActivityStatus.Ready || activity.Status == ActivityStatus.Completed) 
            && activity.ParentActivities.Any())
        {
            logger.LogWarning($"[{activity.JobId}]Commessa {activity.Job.Number.ToString("000")}/{activity.Job.Year}: " +
                $"Attività {activity.RowNumber}/{activity.Type!.Name} in stato '{activity.Status}' " +
                $"è dipendenza di altre {activity.ParentActivities.Count()} attività");

            foreach (var parentActivity in activity.ParentActivities)
            {
                if (parentActivity.Status != ActivityStatus.Completed)
                {
                    if (parentActivity.ActivityDependencies.All(a => a.Status == ActivityStatus.Ready || a.Status == ActivityStatus.Completed)
                    && parentActivity.PurchaseOrderDependencies.All(p => p.Status == PurchaseOrderStatus.Completed))
                    {
                        logger.LogWarning($"[{activity.JobId}]-Commessa {activity.Job.Number.ToString("000")}/{activity.Job.Year}: " +
                            $"Attività {parentActivity.RowNumber}/{parentActivity.Type!.Name}: tutte le dipendenze sono evase -> cambio stato '{parentActivity.Status}' -> '{ActivityStatus.InProgress}' ");
                        parentActivity.Status = ActivityStatus.InProgress;
                    }
                    else
                    {
                        logger.LogWarning($"[{activity.JobId}]-Commessa {activity.Job.Number.ToString("000")}/{activity.Job.Year}: " +
                            $"Attività {parentActivity.RowNumber}/{parentActivity.Type!.Name}: non tutte le dipendenze sono evase -> stato '{parentActivity.Status}' invariato");
                    }
                }
            }
        }

        //check if has dependencies
        if ((activity.Status == ActivityStatus.Ready)
            && (activity.Type!.HasDependencies == true)
            && (activity.ActivityDependencies.Any() || activity.PurchaseOrderDependencies.Any()))
        {
            logger.LogWarning($"[{activity.JobId}]Commessa {activity.Job.Number.ToString("000")}/{activity.Job.Year}: " +
                $"Attività {activity.RowNumber}/{activity.Type!.Name} in stato '{activity.Status}': " +
                $"impostazione evasione dipendenze");

            if (activity.ActivityDependencies.Any())
            {
                foreach (var dep in activity.ActivityDependencies)
                {
                    if (activity.Status == ActivityStatus.Ready)
                    {
                        logger.LogWarning($"[{activity.JobId}]-Commessa {activity.Job.Number.ToString("000")}/{activity.Job.Year}: " +
                        $"Attività {dep.RowNumber}/{dep.Type!.Name}: cambio stato '{dep.Status}' -> '{ActivityStatus.Completed}' ");
                        dep.Status = ActivityStatus.Completed;
                    }
                }
            }

            //if (activity.PurchaseOrderDependencies.Any())
            //{
            //    foreach (var dep in activity.PurchaseOrderDependencies)
            //    {
            //        logger.LogWarning($"[{activity.JobId}]-Commessa {activity.Job.Number.ToString("000")}/{activity.Job.Year}: " +
            //            $"Ordine d'acquisto {dep.Number}: cambio stato '{dep.Status}' -> '{PurchaseOrderStatus.Completed}' ");
            //        dep.Status = PurchaseOrderStatus.Completed;
            //    }
            //}

        }

        repository.Update(activity);
        await dbContext.SaveChanges();

        if ((bool)activity.Type.InfluenceJobStatus && activity.JobId != null)
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
                    var PreviousStatus = job.Status;
                    Func<Job, JobStatus> statusDelegate = StatusExpression.Compile();
                    job.Status = statusDelegate(job);
                    logger.LogWarning($"[{activity.JobId}]Commessa {job.Number.ToString("000")}/{job.Year}: " +
                        $"modifica attività {activity.RowNumber}/{activity.Type!.Name}: " +
                        $"cambio stato commessa '{PreviousStatus}' -> '{job.Status}' ");
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
            var PreviousStatus = ticket.Status;
            ticket.Status = TicketStatus.Resolved;
            logger.LogWarning($"[{activity.JobId}]Commessa {activity.Job.Number.ToString("000")}/{activity.Job.Year}: " +
                $"modifica attività {activity.RowNumber}/{activity.Type!.Name}: " +
                $"cambio stato ticket {ticket.Number.ToString("000")}/{ticket.Year} '{PreviousStatus}' -> '{ticket.Status}' ");
            ticketRepository.Update(ticket);
            await dbContext.SaveChanges();
        }


        if (IsFloorDeliveryChanged && activity.IsFloorDelivery == true)
        {
            await HandleFloorDeliveryAsync(activity);
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
            .Include(e => e.Job)
            .Where(e => e.Id == id)
            .FirstAsync();
        var activityProducts = activityProductRepository.Query()
            .Where(e => e.ActivityId == id);
        var products = productRepository.Query()
            .Where(e => e.CustomerId == activity.Job!.CustomerId && e.IsDecommissioned == false);
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

    public async Task AssignAllCustomerProductsMonthlyMaint(long id)
    {
        var activity = await repository.Query()
            .AsNoTracking()
            .Include(e => e.Job)
            .Where(e => e.Id == id)
            .FirstAsync();
        var activityProducts = activityProductRepository.Query()
            .Where(e => e.ActivityId == id);
        var products = productRepository.Query()
            .Where(e => e.CustomerId == activity.Job!.CustomerId && e.MonthlyMaintenance == true && e.IsDecommissioned == false);
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
                    Order = group.First().Type.Order,
                    Active = group.Where(e => e.ExpirationDate >= DateTimeOffset.UtcNow.Date).Count(),
                    Expired = group.Where(e => e.ExpirationDate < DateTimeOffset.UtcNow.Date).Count()
                })
                //.Project<ActivityCounterDto>(mapper)
                .OrderBy(e => e.Order)
                .ThenBy(e => e.ActivityName)
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

    public async Task<ActivityDto> CopyActivity(CopyDto copyDto)
    {
        var sourceActivity = await repository.Query()
            .AsNoTracking()
            .Where(x => x.Id == copyDto.SourceId)
            .Include(x => x.Attachments)
            .FirstOrDefaultAsync();
        
        var number = await GetNextNumber(copyDto.JobId);
        Activity activity = new Activity();

        activity.SetNumber(number);
        activity.IsNewReferent = true;
        activity.JobId = copyDto.JobId;
        activity.Description = sourceActivity.Description;
        activity.ExpirationDate = sourceActivity.ExpirationDate;
        activity.Informations = sourceActivity.Informations;
        activity.ReferentId = sourceActivity.ReferentId;
        activity.ShortDescription = sourceActivity.ShortDescription;
        activity.StartDate = sourceActivity.StartDate;
        activity.Status = ActivityStatus.Pending;
        activity.SupplierId = sourceActivity.SupplierId;
        activity.TypeId = sourceActivity.TypeId;
        foreach (var attachment in sourceActivity.Attachments)
        {
            activity.Attachments.Add(new ActivityAttachment()
            {
                FileName = attachment.FileName,
                DisplayName = attachment.DisplayName
            });
        }

        await repository.Insert(activity);

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
                    try
                    {
                        var PreviousStatus = job.Status;
                        Func<Job, JobStatus> statusDelegate = StatusExpression.Compile();
                        job.Status = statusDelegate(job);
                        logger.LogWarning($"[{activity.JobId}]Commessa {job.Number.ToString("000")}/{job.Year}: " +
                            $"copia attività {activity.RowNumber}/{activity.Type!.Name}: " +
                            $"cambio stato commessa '{PreviousStatus}' -> '{job.Status}' ");
                        jobRepository.Update(job);
                        await dbContext.SaveChanges();
                    }
                    catch (Exception ex) { }
                }
            }
        }

        return await Get(activity.Id);
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

    public IQueryable<ActivityReadModel> GetJobActivities(long jobId)
    {
        var activities = repository
            .Query()
            .AsNoTracking()
            .Where(e => e.JobId == jobId && e.Type!.HasDependencies != true);

        return activities
            .AsQueryable()
            .Project<ActivityReadModel>(mapper);
    }

    public async Task UpdateDependencies(long id, DependencyDto dependencyDto)
    {
        var activity = await repository.Query()
            .Include(a => a.ActivityDependencies)
            .Include(a => a.PurchaseOrderDependencies)
            .FirstOrDefaultAsync(a => a.Id == id);

        if (activity == null)
        {
            throw new NotFoundException($"Attività con Id {id} non trovata.");
        }

        activity.ActivityDependencies.Clear();
        if (dependencyDto.ActivityDependenciesId.Any())
        {
            var dependencies = await repository.Query()
                .Where(a => dependencyDto.ActivityDependenciesId.Contains(a.Id))
                .ToListAsync();

            foreach (var dep in dependencies)
            {
                activity.ActivityDependencies.Add(dep);
            }
        }

        activity.PurchaseOrderDependencies.Clear();
        if (dependencyDto.PurchaseOrderDependenciesId.Any())
        {
            var purchaseOrders = await purchaseOrderRepository.Query()
                .Where(po => dependencyDto.PurchaseOrderDependenciesId.Contains(po.Id))
                .ToListAsync();

            foreach (var po in purchaseOrders)
            {
                activity.PurchaseOrderDependencies.Add(po);
            }
        }

        repository.Update(activity);
        await dbContext.SaveChanges();
    }
    public async Task<DependencyDto> GetDependencies(long id)
    {
        var activity = await repository.Query()
            .Include(a => a.ActivityDependencies)
            .Include(a => a.PurchaseOrderDependencies)
            .FirstOrDefaultAsync(a => a.Id == id);

        if (activity == null)
        {
            throw new NotFoundException($"Attività con Id {id} non trovata.");
        }

        var dto = new DependencyDto
        {
            ActivityDependenciesId = activity.ActivityDependencies.Select(a => a.Id).ToList(),
            PurchaseOrderDependenciesId = activity.PurchaseOrderDependencies.Select(po => po.Id).ToList()
        };

        return dto;
    }

    public async Task<FloorDeliverySettings> GetFloorDeliverySettings()
    {
        FloorDeliverySettings settings = new FloorDeliverySettings();

        settings.SupplierId = (long)await globalSettingRepository.Query()
            .AsNoTracking()
            .Where(s => s.Type == GlobalSettingType.FloorDeliverySupplierId)
            .Select(s => s.ValueNumber)
            .FirstOrDefaultAsync();

        settings.Amount = (long)await globalSettingRepository.Query()
            .AsNoTracking()
            .Where(s => s.Type == GlobalSettingType.FloorDeliveryExpenseAmount)
            .Select(s => s.ValueNumber)
            .FirstOrDefaultAsync();

        settings.Note = await globalSettingRepository.Query()
            .AsNoTracking()
            .Where(s => s.Type == GlobalSettingType.FloorDeliveryExpenseNote)
            .Select(s => s.ValueString.ToString())
            .FirstOrDefaultAsync();

        return settings;
    }

    private async Task HandleFloorDeliveryAsync(Activity activity)
    {
        if (activity == null) return;

        if (activity.IsFloorDelivery != true)
        {
            return;
        }

        var floorDeliverySettings = await GetFloorDeliverySettings();

        // cerca ordine esistente tra le dipendenze dell'attività
        var existingPO = activity.PurchaseOrderDependencies
            .FirstOrDefault(e => e.SupplierId == floorDeliverySettings.SupplierId);

        if (existingPO != null)
        {
            var hasFloorDeliveryExpenses = await purchaseOrderExpenseRepository.Query()
                .Where(e => e.PurchaseOrderId == existingPO.Id && e.Note == floorDeliverySettings.Note)
                .AnyAsync();

            if (!hasFloorDeliveryExpenses)
            {
                var purchaseOrderExpense = new PurchaseOrderExpense
                {
                    PurchaseOrderId = existingPO.Id,
                    Note = floorDeliverySettings.Note,
                    Amount = floorDeliverySettings.Amount,
                    Quantity = 1,
                    JobId = activity.JobId
                };
                await purchaseOrderExpenseRepository.Insert(purchaseOrderExpense);
                await dbContext.SaveChanges();
            }

            return;
        }

        // crea nuovo ordine d'acquisto tramite servizio
        var purchaseOrder = new PurchaseOrderDto
        {
            Id = 0,
            Date = DateTimeOffset.UtcNow,
            Year = DateTimeOffset.UtcNow.Year,
            SupplierId = floorDeliverySettings.SupplierId,
            Description = $"Spese consegna a piano - Attività {activity.ShortDescription}",
            Status = PurchaseOrderStatus.Pending,
            OperatorId = session.CurrentUser!.OperatorId,
            ActivityTypeId = activity.TypeId,
            Jobs = new List<long> { activity.JobId },
            Items = new List<PurchaseOrderItemDto>(),
            Attachments = new List<PurchaseOrderAttachmentDto>(),
            Messages = new List<MessageReadModel>(),
            Expenses = new List<PurchaseOrderExpenseDto>
            {
                new PurchaseOrderExpenseDto
                {
                    Id = 0,
                    JobId = activity.JobId,
                    Note = floorDeliverySettings.Note,
                    Amount = floorDeliverySettings.Amount,
                    Quantity = 1
                }
            }
        };

        var newPurchaseOrder = await purchaseOrdersService.Create(purchaseOrder);
        if (newPurchaseOrder != null)
        {
            var po = await purchaseOrderRepository.Get(newPurchaseOrder.Id);
            if (po != null)
            {
                activity.PurchaseOrderDependencies.Add(po);
                repository.Update(activity);
                await dbContext.SaveChanges();
            }
        }
    }
}

public class FloorDeliverySettings
{
    public long SupplierId { get; set; }
    public long Amount { get; set; }
    public string Note { get; set; }
}