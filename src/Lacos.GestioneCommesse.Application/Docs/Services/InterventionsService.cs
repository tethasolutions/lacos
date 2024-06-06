﻿using AutoMapper;
using Lacos.GestioneCommesse.Application.Docs.DTOs;
using Lacos.GestioneCommesse.Dal;
using Lacos.GestioneCommesse.Domain.Docs;
using Lacos.GestioneCommesse.Domain.Registry;
using Lacos.GestioneCommesse.Domain.Security;
using Lacos.GestioneCommesse.Framework.Exceptions;
using Lacos.GestioneCommesse.Framework.Extensions;
using Lacos.GestioneCommesse.Framework.Session;
using Microsoft.EntityFrameworkCore;
using Telerik.Reporting.Processing;
using Telerik.Reporting;
using Parameter = Telerik.Reporting.Parameter;
using Westcar.WebApplication.Dal;

namespace Lacos.GestioneCommesse.Application.Docs.Services;

public class InterventionsService : IInterventionsService
{
    private readonly IMapper mapper;
    private readonly IRepository<Intervention> repository;
    private readonly IRepository<Operator> operatorRepository;
    private readonly IRepository<ActivityProduct> activityProductRepository;
    private readonly IRepository<Activity> activityRepository;
    private readonly IRepository<InterventionProductCheckList> productCheckListRepository;
    private readonly IRepository<InterventionNote> noteRepository;
    private readonly IViewRepository<InterventionProductCheckListItemKO> productCheckListItemKORepository;
    private readonly ILacosSession session;
    private readonly ILacosDbContext dbContext;

    public InterventionsService(
        IMapper mapper,
        IRepository<Intervention> repository,
        IRepository<Operator> operatorRepository,
        ILacosDbContext dbContext,
        IRepository<ActivityProduct> activityProductRepository,
        IRepository<Activity> activityRepository,
        IRepository<InterventionProductCheckList> productCheckListRepository,
        IRepository<InterventionNote> noteRepository,
        IViewRepository<InterventionProductCheckListItemKO> productCheckListItemKORepository,
        ILacosSession session
    )
    {
        this.mapper = mapper;
        this.repository = repository;
        this.operatorRepository = operatorRepository;
        this.dbContext = dbContext;
        this.activityProductRepository = activityProductRepository;
        this.activityRepository = activityRepository;
        this.productCheckListRepository = productCheckListRepository;
        this.noteRepository = noteRepository;
        this.productCheckListItemKORepository = productCheckListItemKORepository;
        this.session = session;
    }

    public IQueryable<InterventionReadModel> Query()
    {
        var query = repository.Query();

        if (session.IsAuthorized(Role.Operator))
        {
            var user = session.CurrentUser!;

            query = query
                .Where(i =>
                    i.Activity!.Type!.Operators.Any(o => o.Id == user.OperatorId) ||
                    i.Operators.Any(o => o.Id == user.OperatorId)
                );
        }

        return query
            .Project<InterventionReadModel>(mapper);
    }

    public async Task<InterventionDto> Get(long id)
    {
        var interventionDto = await repository.Query()
            .Where(e => e.Id == id)
            .Project<InterventionDto>(mapper)
            .FirstOrDefaultAsync();

        if (interventionDto == null)
        {
            throw new NotFoundException($"Intervento con Id {id} non trovato.");
        }

        return interventionDto;
    }

    public async Task<InterventionDto> Create(InterventionDto interventionDto)
    {
        var intervention = interventionDto.MapTo<Intervention>(mapper);

        await using (var transaction = await dbContext.BeginTransaction())
        {
            await MergeInterventionOperators(intervention, interventionDto.Operators);
            await MergeInterventionProducts(intervention, interventionDto.ActivityProducts);

            await repository.Insert(intervention);

            await dbContext.SaveChanges();

            await UpdateActivityStatus(intervention.ActivityId);

            await transaction.CommitAsync();
        }

        return await Get(intervention.Id);
    }

    public async Task<InterventionDto> Update(InterventionDto interventionDto)
    {
        var intervention = await repository.Query()
            .Include(e => e.Operators)
            .Include(e => e.Products)
            .ThenInclude(e => e.CheckList)
            .ThenInclude(e => e!.Items)
            .Where(e => e.Id == interventionDto.Id)
            .FirstOrDefaultAsync();

        if (intervention == null)
        {
            throw new NotFoundException($"Intervento con Id {interventionDto.Id} non trovato.");
        }

        //if (intervention.Status != InterventionStatus.Scheduled)
        //{
        //    throw new LacosException("Non puoi modificare un intervento già completato.");
        //}

        var previousActivityId = intervention.ActivityId;
        intervention = interventionDto.MapTo(intervention, mapper);

        await using (var transaction = await dbContext.BeginTransaction())
        {
            await MergeInterventionOperators(intervention, interventionDto.Operators);
            await MergeInterventionProducts(intervention, interventionDto.ActivityProducts);

            repository.Update(intervention);

            await dbContext.SaveChanges();

            await UpdateActivityStatus(intervention.ActivityId);

            if (previousActivityId != intervention.ActivityId)
            {
                await UpdateActivityStatus(previousActivityId);
            }

            await transaction.CommitAsync();
        }

        return await Get(intervention.Id);
    }

    public async Task Delete(long id)
    {
        var intervention = await repository.Query()
            .AsSplitQuery()
            .Include(e => e.Products)
            .ThenInclude(e => e.CheckList)
            .ThenInclude(e => e!.Items)
            .FirstOrDefaultAsync(e => e.Id == id);

        if (intervention == null)
        {
            return;
        }

        if (intervention.IsCompleted())
        {
            throw new LacosException("Non puoi eliminare un intervento già completato.");
        }

        await using (var transaction = await dbContext.BeginTransaction())
        {
            repository.Delete(intervention);

            await dbContext.SaveChanges();

            await UpdateActivityStatus(intervention.ActivityId);

            await transaction.CommitAsync();
        }
    }

    private async Task MergeInterventionOperators(Intervention intervention, IEnumerable<long> operatorIds)
    {
        intervention.Operators.Clear();

        var operators = await operatorRepository.Query()
            .Where(e => operatorIds.Contains(e.Id))
            .ToListAsync();

        intervention.Operators.AddRange(operators);
    }

    private async Task MergeInterventionProducts(Intervention intervention, IEnumerable<long> activityProductIds)
    {
        var ids = activityProductIds.ToList();

        foreach (var product in intervention.Products.ToList())
        {
            if (!ids.Contains(product.ActivityProductId))
            {
                intervention.Products.Remove(product);
            }

            ids.Remove(product.ActivityProductId);
        }

        var productsToAdd = await activityProductRepository.Query()
            .AsNoTracking()
            .AsSplitQuery()
            .Where(e => ids.Contains(e.Id))
            .Select(ap => new InterventionProduct
            {
                ActivityProductId = ap.Id,
                CheckList = ap.Activity!.Type!.CheckLists
                    .Where(cl => cl.ProductTypeId == ap.Product!.ProductTypeId)
                    .Select(cl => new InterventionProductCheckList
                    {
                        Description = cl.Description,
                        Items = cl.Items
                            .Select(i => new InterventionProductCheckListItem
                            {
                                Description = i.Description
                            })
                            .ToList()
                    })
                    .FirstOrDefault()
            })
            .ToListAsync();

        intervention.Products.AddRange(productsToAdd);
    }

    public async Task UpdateActivityStatus(long id)
    {
        var activity = await activityRepository.Query()
            .Where(e => e.Id == id)
            .Select(e => new
            {
                CurrentStatus = e.Status,
                Status = !e.Interventions.Any()
                    ? ActivityStatus.Pending
                    : e.Interventions.Any(i => i.Status == InterventionStatus.Scheduled)
                        ? ActivityStatus.InProgress
                        : ActivityStatus.Completed
            })
            .FirstAsync();

        if (activity.Status == activity.CurrentStatus)
        {
            return;
        }

        await activityRepository.Update(id, e => e.Status = activity.Status);

        await dbContext.SaveChanges();
    }

    public IQueryable<InterventionProductReadModel> GetProductsByIntervention(long id)
    {
        var products = repository
            .Query()
            .AsNoTracking()
            .Where(e => e.Id == id)
            .SelectMany(e => e.Products)
            .Project<InterventionProductReadModel>(mapper);
        return products;
    }

    public async Task<InterventionProductCheckListDto> GetInterventionProductCheckList(long interventionProductId)
    {
        var productCheckList = await productCheckListRepository
            .Query()
            .AsNoTracking()
            .Where(e => e.InterventionProductId == interventionProductId)
            .Include(e => e.Items)
            .Project<InterventionProductCheckListDto>(mapper)
            .FirstOrDefaultAsync();

        if (productCheckList == null)
        {
            throw new NotFoundException($"Prodotto con Id {interventionProductId} non trovato.");
        }

        return productCheckList;
    }

    public Task<ReportDto> GenerateReport(long interventionId)
    {
        var parameters = new[] { new Parameter("InterventionId", interventionId) };
        var content = Report("Intervento.trdp", parameters);

        return Task.FromResult(new ReportDto(content, "intervento.pdf"));
    }

    private static byte[] Report(string reportName, params Parameter[] parameters)
    {
        var processor = new ReportProcessor();
        var src = new UriReportSource { Uri = $@"Reports\{reportName}" };

        src.Parameters.AddRange(parameters);

        var result = processor.RenderReport("PDF", src, null);

        return result.DocumentBytes;
    }

    public async Task<InterventionNoteDto> DownloadInterventionNote(string filename)
    {
        var interventionNote = await noteRepository
            .Query()
            .AsNoTracking()
            .Where(x => x.PictureFileName == filename)
            .Project<InterventionNoteDto>(mapper)
            .SingleOrDefaultAsync();

        if (interventionNote == null)
        {
            throw new NotFoundException($"Nota con Id {filename} non trovata.");
        }

        return interventionNote;
    }

    public async Task<IEnumerable<InterventionNoteDto>> GetInterventionAttachments(long jobId, long activityId)
    {
        if (activityId != 0)
        {
            var interventionActivityAttachments = await noteRepository
                .Query()
                .AsNoTracking()
                .Where(x => x.Intervention.Activity.Id == activityId)
                .OrderBy(x => x.CreatedOn)
                .ToArrayAsync();

            return interventionActivityAttachments.MapTo<IEnumerable<InterventionNoteDto>>(mapper);
        }

        var interventionAttachments = await noteRepository
            .Query()
            .AsNoTracking()
            .Where(x => x.Intervention.Activity.JobId == jobId)
            .OrderBy(x => x.CreatedOn)
            .ToArrayAsync();

        return interventionAttachments.MapTo<IEnumerable<InterventionNoteDto>>(mapper);
    }

    public IQueryable<InterventionProductCheckListItemKOReadModel> GetInterventionsKo()
    {
        var query = productCheckListItemKORepository.Query();

        return query
            .Project<InterventionProductCheckListItemKOReadModel>(mapper);
    }

}