using AutoMapper;
using Lacos.GestioneCommesse.Application.Docs.DTOs;
using Lacos.GestioneCommesse.Dal;
using Lacos.GestioneCommesse.Domain.Docs;
using Lacos.GestioneCommesse.Domain.Registry;
using Lacos.GestioneCommesse.Framework.Exceptions;
using Lacos.GestioneCommesse.Framework.Extensions;
using Lacos.GestioneCommesse.Framework.Session;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Linq.Expressions;
using Westcar.WebApplication.Dal;

namespace Lacos.GestioneCommesse.Application.Docs.Services;

public class PurchaseOrdersService : IPurchaseOrdersService
{
    private readonly IMapper mapper;
    private readonly IRepository<PurchaseOrder> repository;
    private readonly IViewRepository<PurchaseOrderSummary> summaryRepository;
    private readonly IRepository<PurchaseOrderItem> repositoryItem;
    private readonly IRepository<PurchaseOrderAttachment> purchaseOrderAttachmentRepository;
    private readonly IRepository<Job> jobRepository;
    private readonly IRepository<Domain.Docs.Activity> activityRepository;
    private readonly IRepository<Operator> operatorRepository;
    private readonly ILacosSession session;
    private readonly ILacosDbContext dbContext;
    private readonly ILogger<ActivitiesService> logger;
    private readonly IMessagesService messagesService;

    private static readonly Expression<Func<Job, JobStatus>> StatusExpression = j =>
        j.PurchaseOrders.Any(a => a.Status == PurchaseOrderStatus.Ordered || a.Status == PurchaseOrderStatus.Partial || a.Status == PurchaseOrderStatus.Completed)
            ? JobStatus.InProgress
            : j.Status;

    public PurchaseOrdersService(
        IMapper mapper,
        IRepository<PurchaseOrder> repository,
        IViewRepository<PurchaseOrderSummary> summaryRepository,
        IRepository<PurchaseOrderItem> repositoryItem,
        IRepository<PurchaseOrderAttachment> purchaseOrderAttachmentRepository,
        IRepository<Job> jobRepository,
        IRepository<Domain.Docs.Activity> activityRepository,
        IRepository<Operator> operatorRepository,
        ILacosSession session,
        ILacosDbContext dbContext,
        ILogger<ActivitiesService> logger,
        IMessagesService messagesService
    )
    {
        this.mapper = mapper;
        this.repository = repository;
        this.summaryRepository = summaryRepository;
        this.repositoryItem = repositoryItem;
        this.purchaseOrderAttachmentRepository = purchaseOrderAttachmentRepository;
        this.jobRepository = jobRepository;
        this.activityRepository = activityRepository;
        this.operatorRepository = operatorRepository;
        this.session = session;
        this.dbContext = dbContext;
        this.logger = logger;
        this.messagesService = messagesService;
    }

    public IQueryable<PurchaseOrderSummary> Query(long? jobId)
    {
        if (jobId != null)
        { 
            var filtered = summaryRepository.Query()
                .AsNoTracking()
                .ToList()
                .Where(e => e.JobIds?.Split(',').Contains(jobId.ToString()) ?? false)
                .AsQueryable();
            return filtered;
        }

        var query = summaryRepository.Query()
            .AsNoTracking();

        return query;
    }

    public async Task<PurchaseOrderDto> Get(long id)
    {
        var purchaseOrderDto = await dbContext.ExecuteWithDisabledQueryFilters(async () => await repository.Query()
            .Where(e => e.Id == id)
            .Project<PurchaseOrderDto>(mapper)
            .FirstOrDefaultAsync(), QueryFilter.OperatorEntity);

        //purchaseOrderDto.Messages = purchaseOrderDto.Messages.Where(m => m.OperatorId == session.CurrentUser.OperatorId ||
        //    m.TargetOperatorsId.Contains(session.CurrentUser.OperatorId.ToString()));

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

        if (purchaseOrderDto.Jobs != null)
        {
            foreach (long jobId in purchaseOrderDto.Jobs)
            { 
                var job = await jobRepository.Query()
                    .Where(e => e.Id == jobId)
                    .FirstOrDefaultAsync();
                purchaseOrder.Jobs.Add(job);
            }
        }

        await repository.Insert(purchaseOrder);

        foreach (var file in purchaseOrder.Attachments)
        {
            var purchaseOrderAttachment = file.MapTo<PurchaseOrderAttachment>(mapper);
            purchaseOrderAttachment.PurchaseOrderId = purchaseOrder.Id;
            purchaseOrderAttachment.PurchaseOrder = purchaseOrder;
            purchaseOrderAttachment.DisplayName = file.DisplayName;
            purchaseOrderAttachment.FileName = file.FileName;

            await purchaseOrderAttachmentRepository.Insert(purchaseOrderAttachment);
        }

        await dbContext.SaveChanges();

        return await Get(purchaseOrder.Id);
    }

    public async Task<PurchaseOrderDto> Update(PurchaseOrderDto purchaseOrderDto)
    {
        var purchaseOrder = await repository.Query()
            .Where(x => x.Id == purchaseOrderDto.Id)
            .Include(x => x.Supplier)
            .Include(x => x.Items)
            .Include(x => x.Attachments)
            .Include(x => x.Messages)
            .Include(x => x.Jobs)
            .Include(x => x.ParentActivities)
            .ThenInclude(y => y.Type)
            .Include(x => x.ParentActivities)
            .ThenInclude(y => y.ActivityDependencies)
            .Include(x => x.ParentActivities)
            .ThenInclude(y => y.PurchaseOrderDependencies)
            .FirstOrDefaultAsync();

        if (purchaseOrder == null)
        {
            throw new NotFoundException($"Ordine con Id {purchaseOrderDto.Id} non trovato.");
        }

        if (purchaseOrderDto.Jobs != null)
        {
            purchaseOrder.Jobs.Clear();
            foreach (long jobId in purchaseOrderDto.Jobs)
            {
                var job = await jobRepository.Query()
                    .Where(e => e.Id == jobId)
                    .FirstOrDefaultAsync();
                purchaseOrder.Jobs.Add(job);
            }
        }

        //quando viene evaso l'ordine viene creato un nuovo commento al creatore dell'ordine
        bool isChangedAsCompleted = (purchaseOrder.Status != purchaseOrderDto.Status && purchaseOrderDto.Status == PurchaseOrderStatus.Completed);
        bool isChangedAsPartialCompleted = (purchaseOrder.Status != purchaseOrderDto.Status && purchaseOrderDto.Status == PurchaseOrderStatus.Partial);

        purchaseOrder = purchaseOrderDto.MapTo(purchaseOrder, mapper);

        //check if has parent activities
        if ((purchaseOrder.Status == PurchaseOrderStatus.Completed)
            && purchaseOrder.ParentActivities.Any())
            {
                foreach (var job in purchaseOrder.Jobs)
                {
                logger.LogWarning($"[{job.Id}]Commessa {job.Number.ToString("000")}/{job.Year}: " +
                    $"Ordine del {purchaseOrder.Date.ToString("dd/MM/yy")} in stato '{purchaseOrder.Status}' " +
                    $"è dipendenza di altre {purchaseOrder.ParentActivities.Count()} attività");

                foreach (var parentActivity in purchaseOrder.ParentActivities)
                {
                    if (parentActivity.Status != ActivityStatus.Completed)
                    {
                        if (parentActivity.ActivityDependencies.All(a => a.Status == ActivityStatus.Ready || a.Status == ActivityStatus.Completed)
                            && parentActivity.PurchaseOrderDependencies.All(p => p.Status == PurchaseOrderStatus.Completed))
                        {
                            logger.LogWarning($"[{job.Id}]-Commessa {job.Number.ToString("000")}/{job.Year}: " +
                                $"Attività {parentActivity.RowNumber}/{parentActivity.Type!.Name}: tutte le dipendenze sono evase -> cambio stato '{parentActivity.Status}' -> '{ActivityStatus.InProgress}' ");
                            parentActivity.Status = ActivityStatus.InProgress;
                        }
                        else
                        {
                            logger.LogWarning($"[{job.Id}]-Commessa {job.Number.ToString("000")}/{job.Year}: " +
                                $"Attività {parentActivity.RowNumber}/{parentActivity.Type!.Name}: non tutte le dipendenze sono evase -> stato '{parentActivity.Status}' invariato");
                        }
                    }
                }
            }
        }

        repository.Update(purchaseOrder);

        await dbContext.SaveChanges();

        //update job status
        foreach (var j in purchaseOrder.Jobs)
        {
            Job job = await jobRepository.Query()
                .Where(e => e.Id == j.Id)
                .Include(e => e.PurchaseOrders)
                .FirstOrDefaultAsync();
            if (job != null)
            {
                if (job.Status == JobStatus.Pending)
                {
                    var PreviousStatus = job.Status;
                    Func<Job, JobStatus> statusDelegate = StatusExpression.Compile();
                    job.Status = statusDelegate(job);
                    logger.LogWarning($"[{job.Id}]Commessa {job.Number.ToString("000")}/{job.Year}: " +
                        $"modifica ordine d'acquisto {purchaseOrder.Number}/{purchaseOrder.Year}: " +
                        $"cambio stato commessa '{PreviousStatus}' -> '{job.Status}' ");
                    jobRepository.Update(job);
                    await dbContext.SaveChanges();
                }
            }
        }

        //quando viene evaso l'ordine viene creato un nuovo commento al creatore dell'ordine
        if (isChangedAsCompleted || isChangedAsPartialCompleted)
        {
            var partialText = isChangedAsPartialCompleted ? "parzialmente " : "";

            var message = new MessageDto()
            {
                OperatorId = (long)session.CurrentUser.OperatorId,
                Date = DateTimeOffset.Now,
                Note = $"L'ordine {purchaseOrder.Number} del fornitore {purchaseOrder.Supplier.Name} è stato {partialText}consegnato",
                IsFromApp = true
            };

            var targetOperatorId = await operatorRepository.Query()
                    .Where(o => o.UserId == purchaseOrder.CreatedById)
                    .Select(o => o.Id)
                    .FirstOrDefaultAsync();

            if (targetOperatorId != null)
            {
                if (purchaseOrder.ParentActivities.Any())
                { 
                    foreach(var activity in purchaseOrder.ParentActivities)
                    {
                        message.ActivityId = activity.Id;
                        await messagesService.Create(message, targetOperatorId.ToString());
                    }
                }
                else if (purchaseOrder.ActivityTypeId == null)
                {
                    message.PurchaseOrderId = purchaseOrder.Id;
                    await messagesService.Create(message, targetOperatorId.ToString());
                }
                else
                {
                    var jobIds = purchaseOrder.Jobs.Select(j => j.Id).ToList();
                    var activities = await activityRepository.Query()
                        .Where(a => jobIds.Contains(a.JobId) && a.TypeId == purchaseOrder.ActivityTypeId)
                        .ToListAsync();
                    foreach (var activity in activities)
                    {
                        message.ActivityId = activity.Id;
                        await messagesService.Create(message, targetOperatorId.ToString());
                    }
                }
            }
        }

        return await Get(purchaseOrder.Id);
    }

    public async Task<PurchaseOrderDto> CopyPurchaseOrder(CopyDto copyDto)
    {
        var sourcePurchaseOrder = await repository.Query()
            .AsNoTracking()
            .Where(x => x.Id == copyDto.SourceId)
            .Include(x => x.Attachments)
            .Include(x => x.Items)
            .FirstOrDefaultAsync();

        PurchaseOrder purchaseOrder = new PurchaseOrder();

        purchaseOrder.Number = await GetNextNumber(DateTimeOffset.Now.Year);
        purchaseOrder.Date = DateTimeOffset.Now.Date;
        purchaseOrder.Year = DateTimeOffset.Now.Year;
        purchaseOrder.Jobs.Add(await jobRepository.Get(copyDto.JobId));
        purchaseOrder.Description = sourcePurchaseOrder.Description;
        purchaseOrder.Items = sourcePurchaseOrder.Items;
        purchaseOrder.Status = PurchaseOrderStatus.Pending;
        purchaseOrder.SupplierId = sourcePurchaseOrder.SupplierId;
        purchaseOrder.ActivityTypeId = sourcePurchaseOrder.ActivityTypeId;
        purchaseOrder.OperatorId = session.CurrentUser.OperatorId;
        foreach (var attachment in sourcePurchaseOrder.Attachments)
        {
            purchaseOrder.Attachments.Add(new PurchaseOrderAttachment()
            {
                FileName = attachment.FileName,
                DisplayName = attachment.DisplayName,
                IsAdminDocument = attachment.IsAdminDocument
            });
        }

        await repository.Insert(purchaseOrder);

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

    // --------------------------------------------------------------------------------------------------------------
    public async Task<IEnumerable<PurchaseOrderAttachmentReadModel>> GetPurchaseOrderAttachments(long jobId, long purchaseOrderId)
    {
        if (purchaseOrderId != 0)
        {
            var purchaseOrderAttachments = await purchaseOrderAttachmentRepository
                .Query()
                .AsNoTracking()
                .Where(x => x.PurchaseOrder.Id == purchaseOrderId)
                .OrderBy(x => x.CreatedOn)
                .ToArrayAsync();

            return purchaseOrderAttachments.MapTo<IEnumerable<PurchaseOrderAttachmentReadModel>>(mapper);
        }

        var job = await jobRepository.Get(jobId);

        var purchaseOrdersAttachments = await purchaseOrderAttachmentRepository
            .Query()
            .AsNoTracking()
            .Where(x => x.PurchaseOrder.Jobs.Contains(job))
            .OrderBy(x => x.CreatedOn)
            .ToArrayAsync();

        return purchaseOrdersAttachments.MapTo<IEnumerable<PurchaseOrderAttachmentReadModel>>(mapper);
    }

    public async Task<PurchaseOrderAttachmentReadModel> GetPurchaseOrderAttachmentDetail(long attachmentId)
    {
        var purchaseOrderAttachment = await purchaseOrderAttachmentRepository
            .Query()
            .AsNoTracking()
            .Where(x => x.Id == attachmentId)
            .SingleOrDefaultAsync();

        return purchaseOrderAttachment.MapTo<PurchaseOrderAttachmentReadModel>(mapper);
    }

    public async Task<PurchaseOrderAttachmentReadModel> DownloadPurchaseOrderAttachment(string filename)
    {
        var purchaseOrderAttachment = await purchaseOrderAttachmentRepository
            .Query()
            .AsNoTracking()
            .Where(x => x.FileName == filename)
            .SingleOrDefaultAsync();

        return purchaseOrderAttachment.MapTo<PurchaseOrderAttachmentReadModel>(mapper);
    }

    public async Task<PurchaseOrderAttachmentDto> UpdatePurchaseOrderAttachment(long id, PurchaseOrderAttachmentDto attachmentDto)
    {
        var attachment = await purchaseOrderAttachmentRepository.Get(id);

        if (attachment == null)
        {
            throw new NotFoundException("Errore allegato");
        }
        attachmentDto.MapTo(attachment, mapper);

        purchaseOrderAttachmentRepository.Update(attachment);

        await dbContext.SaveChanges();

        return attachment.MapTo<PurchaseOrderAttachmentDto>(mapper);
    }

    public async Task<PurchaseOrderAttachmentDto> CreatePurchaseOrderAttachment(PurchaseOrderAttachmentDto attachmentDto)
    {
        var attachment = attachmentDto.MapTo<PurchaseOrderAttachment>(mapper);

        await purchaseOrderAttachmentRepository.Insert(attachment);

        await dbContext.SaveChanges();

        return attachment.MapTo<PurchaseOrderAttachmentDto>(mapper);
    }

    //------------------------------------------------------------------------------------------------------------
    public IQueryable<PurchaseOrderSummary> GetJobPurchaseOrders(long jobId)
    {
        var query = summaryRepository.Query()
            .AsNoTracking()
            .ToList()
            .Where(e => e.JobIds?.Split(',').Contains(jobId.ToString()) ?? false)
            .AsQueryable();

        return query;

        //var job = jobRepository.Query().FirstOrDefault(j => j.Id == jobId);

        //if (job == null)
        //{
        //    throw new NotFoundException($"Commessa con Id {jobId} non trovata.");
        //}

        //var query = repository
        //    .Query()
        //    .AsNoTracking()
        //    .Where(e => e.Jobs.Contains(job));

        //return query
        //    .AsQueryable()
        //    .Project<PurchaseOrderReadModel>(mapper);
    }
}