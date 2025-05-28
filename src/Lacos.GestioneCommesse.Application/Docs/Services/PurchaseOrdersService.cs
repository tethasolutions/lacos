using AutoMapper;
using Lacos.GestioneCommesse.Application.Docs.DTOs;
using Lacos.GestioneCommesse.Dal;
using Lacos.GestioneCommesse.Domain.Docs;
using Lacos.GestioneCommesse.Domain.Security;
using Lacos.GestioneCommesse.Framework.Exceptions;
using Lacos.GestioneCommesse.Framework.Extensions;
using Lacos.GestioneCommesse.Framework.Session;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace Lacos.GestioneCommesse.Application.Docs.Services;

public class PurchaseOrdersService : IPurchaseOrdersService
{
    private readonly IMapper mapper;
    private readonly IRepository<PurchaseOrder> repository;
    private readonly IRepository<PurchaseOrderItem> repositoryItem;
    private readonly IRepository<PurchaseOrderAttachment> purchaseOrderAttachmentRepository;
    private readonly ILacosSession session;
    private readonly ILacosDbContext dbContext;
    private readonly ILogger<ActivitiesService> logger;

    public PurchaseOrdersService(
        IMapper mapper,
        IRepository<PurchaseOrder> repository,
        IRepository<PurchaseOrderItem> repositoryItem,
        IRepository<PurchaseOrderAttachment> purchaseOrderAttachmentRepository,
        ILacosSession session,
        ILacosDbContext dbContext,
        ILogger<ActivitiesService> logger
    )
    {
        this.mapper = mapper;
        this.repository = repository;
        this.repositoryItem = repositoryItem;
        this.purchaseOrderAttachmentRepository = purchaseOrderAttachmentRepository;
        this.session = session;
        this.dbContext = dbContext;
        this.logger = logger;
    }

    public IQueryable<PurchaseOrderReadModel> Query()
    {
        var user = session.CurrentUser!;

        return repository.Query()
            .AsNoTracking()
            .Include(x => x.Job)
            .ThenInclude(x => x.Customer)
            //.Where(e => e.ActivityType!.Operators.Any(o => o.Id == user.OperatorId)
            .Project<PurchaseOrderReadModel>(mapper);
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
            .Where(e => e.Id == purchaseOrderDto.Id)
            .Include(e => e.Items)
            .Include(x => x.Attachments)
            .Include(x => x.Messages)
            .Include(x => x.Job)
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

        purchaseOrder = purchaseOrderDto.MapTo(purchaseOrder, mapper);

        //check if has parent activities
        if ((purchaseOrder.Status == PurchaseOrderStatus.Completed)
            && purchaseOrder.ParentActivities.Any())
        {
            logger.LogWarning($"[{purchaseOrder.JobId}]Commessa {purchaseOrder.Job.Number.ToString("000")}/{purchaseOrder.Job.Year}: " +
                $"Ordine del {purchaseOrder.Date.ToString("dd/MM/yy")} in stato '{purchaseOrder.Status}' " +
                $"è dipendenza di altre {purchaseOrder.ParentActivities.Count()} attività");

            foreach (var parentActivity in purchaseOrder.ParentActivities)
            {
                if (parentActivity.ActivityDependencies.All(a => a.Status == ActivityStatus.Ready || a.Status == ActivityStatus.Completed)
                    && parentActivity.PurchaseOrderDependencies.All(p => p.Status == PurchaseOrderStatus.Completed))
                {
                    logger.LogWarning($"[{purchaseOrder.JobId}]-Commessa {purchaseOrder.Job.Number.ToString("000")}/{purchaseOrder.Job.Year}: " +
                        $"Attività {parentActivity.RowNumber}/{parentActivity.Type!.Name}: tutte le dipendenze sono evase -> cambio stato '{parentActivity.Status}' -> '{ActivityStatus.InProgress}' ");
                    parentActivity.Status = ActivityStatus.InProgress;
                }
                else
                {
                    logger.LogWarning($"[{purchaseOrder.JobId}]-Commessa {purchaseOrder.Job.Number.ToString("000")}/{purchaseOrder.Job.Year}: " +
                        $"Attività {parentActivity.RowNumber}/{parentActivity.Type!.Name}: non tutte le dipendenze sono evase -> stato '{parentActivity.Status}' invariato");
                }
            }
        }

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

        var purchaseOrdersAttachments = await purchaseOrderAttachmentRepository
            .Query()
            .AsNoTracking()
            .Where(x => x.PurchaseOrder.JobId == jobId)
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
    public IQueryable<PurchaseOrderReadModel> GetJobPurchaseOrders(long jobId)
    {
        var query = repository
            .Query()
            .AsNoTracking()
            .Where(e => e.JobId == jobId);

        return query
            .AsQueryable()
            .Project<PurchaseOrderReadModel>(mapper);
    }
}