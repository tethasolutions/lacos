using AutoMapper;
using Lacos.GestioneCommesse.Application.Docs.DTOs;
using Lacos.GestioneCommesse.Application.Shared.Services;
using Lacos.GestioneCommesse.Dal;
using Lacos.GestioneCommesse.Domain.Docs;
using Lacos.GestioneCommesse.Domain.Registry;
using Lacos.GestioneCommesse.Domain.Security;
using Lacos.GestioneCommesse.Framework.Exceptions;
using Lacos.GestioneCommesse.Framework.Extensions;
using Lacos.GestioneCommesse.Framework.Session;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;
using Telerik.Reporting;
using Telerik.Reporting.Processing;
using Westcar.WebApplication.Dal;
using Parameter = Telerik.Reporting.Parameter;

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
    private readonly IRepository<Job> jobRepository;
    private readonly IRepository<Ticket> ticketRepository;
    private readonly IRepository<MaintenancePriceListItem> mainentancePriceListItemRepository; 
    private readonly ILacosSession session;
    private readonly ILacosDbContext dbContext;
    private readonly ILogger<ActivitiesService> logger;
    private readonly ISharedService sharedService;

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
        IRepository<Job> jobRepository,
        IRepository<Ticket> ticketRepository,
        IRepository<MaintenancePriceListItem> mainentancePriceListItemRepository,
        ILacosSession session,
        ILogger<ActivitiesService> logger,
        ISharedService sharedService
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
        this.jobRepository = jobRepository;
        this.ticketRepository = ticketRepository;
        this.mainentancePriceListItemRepository = mainentancePriceListItemRepository;
        this.session = session;
        this.logger = logger;
        this.sharedService = sharedService;
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

            var activity = await activityRepository.Query()
                .Include(a => a.Address)
                .Include(a => a.Type)
                .Include(a => a.Job)
                .ThenInclude(a => a.Customer)
                .Include(a => a.Referent)
                .Where(a => a.Id == intervention.ActivityId)
                .FirstOrDefaultAsync();

            if (activity != null)
            {
                if (activity.Type.HasServiceFees)
                {
                    await UpdateInterventionServiceFees(activity, intervention);
                }

                //invio mail creazione intervento
                foreach (var interventionOperator in intervention.Operators)
                {
                    if (interventionOperator.Email == null)
                    {
                        continue;
                    }
                    string address = activity.Address != null ? activity.Address.StreetAddress + " " + activity.Address.ZipCode + " " + activity.Address.City + " " + activity.Address.Province : "";

                    var body = $"<p>Gentile {interventionOperator.Name},</p>" +
                        $"<p>ti informiamo che ti è stato assegnato il seguente intervento:</p>" +
                        $"<ul><li><strong>Cliente:</strong> {activity.Job.Customer.Name}</li>" +
                        $"<li><strong>Luogo:</strong> {address}</li>" +
                        $"<li><strong>Link mappa:</strong> <a href='https://www.google.it/maps/place/{address}'>Apri in Google Maps</a></li>" +
                        $"<li><strong>Data e ora inizio:</strong> {intervention.Start.ToString("dd/MM/yyyy HH:mm")}</li>" +
                        $"<li><strong>Data e ora fine:</strong> {intervention.End.ToString("dd/MM/yyyy HH:mm")}</li>" +
                        $"<li><strong>Tipologia intervento:</strong> {activity.Type.Name}</li>" +
                        $"<li><strong>Descrizione:</strong> {intervention.Description}</li>" +
                        $"</ul>" +
                        $"<p>Ti chiediamo di prendere in carico l’intervento e di aggiornare lo stato secondo le procedure previste, segnalando eventuali criticità o necessità di supporto.</p>" +
//                        $"<p>Per qualsiasi dubbio o informazione aggiuntiva, puoi fare riferimento a <strong>{activity.Referent}</strong>.</p>" +
                        $"<p>Grazie per la collaborazione.<br />" +
                        $"Cordiali saluti<br />" +
                        $"<strong><i>Staff Lacos</i></strong></p>";
                    await sharedService.SendMessage(interventionOperator.Email!, "", "Nuovo intervento assegnato", body);
                }
            }

            await repository.Insert(intervention);
            await dbContext.SaveChanges();

            if (activity != null)
            {
                activity.ExpirationDate = intervention.Start;
                activityRepository.Update(activity);
                await dbContext.SaveChanges();
            }

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

        bool hasScheduledTimeUpdated = (intervention.Start != interventionDto.Start || intervention.End != interventionDto.End);

        var previousActivityId = intervention.ActivityId;
        intervention = interventionDto.MapTo(intervention, mapper);

        await using (var transaction = await dbContext.BeginTransaction())
        {
            await MergeInterventionOperators(intervention, interventionDto.Operators);
            await MergeInterventionProducts(intervention, interventionDto.ActivityProducts);

            if (hasScheduledTimeUpdated)
            {
                var activity = await activityRepository.Query()
                    .Include(a => a.Address)
                    .Include(a => a.Type)
                    .Include(a => a.Job)
                    .ThenInclude(a => a.Customer)
                    .Include(a => a.Referent)
                    .Where(a => a.Id == intervention.ActivityId)
                    .FirstOrDefaultAsync();

                if (activity != null)
                {
                    if (activity.Type.HasServiceFees)
                    {
                        await UpdateInterventionServiceFees(activity, intervention);
                    }

                    if (hasScheduledTimeUpdated)
                    {
                        //invio mail modifica intervento
                        foreach (var interventionOperator in intervention.Operators)
                        {
                            if (interventionOperator.Email == null)
                            {
                                continue;
                            }
                            string address = activity.Address != null ? activity.Address.StreetAddress + " " + activity.Address.ZipCode + " " + activity.Address.City + " " + activity.Address.Province : "";

                            var body = $"<p>Gentile {interventionOperator.Name},</p>" +
                                $"<p>ti informiamo che il seguente intervento ha avuto una variazione di orario:</p>" +
                                $"<ul><li><strong>Cliente:</strong> {activity.Job.Customer.Name}</li>" +
                                $"<li><strong>Luogo:</strong> {address}</li>" +
                                $"<li><strong>Link mappa:</strong> <a href='https://www.google.it/maps/place/{address}'>Apri in Google Maps</a></li>" +
                                $"<li><strong>Data e ora inizio:</strong> {intervention.Start.ToString("dd/MM/yyyy HH:mm")}</li>" +
                                $"<li><strong>Data e ora fine:</strong> {intervention.End.ToString("dd/MM/yyyy HH:mm")}</li>" +
                                $"<li><strong>Tipologia intervento:</strong> {activity.Type.Name}</li>" +
                                $"<li><strong>Descrizione:</strong> {intervention.Description}</li>" +
                                $"</ul>" +
                                $"<p>Ti chiediamo di prendere in carico l’intervento e di aggiornare lo stato secondo le procedure previste, segnalando eventuali criticità o necessità di supporto.</p>" +
                                //$"<p>Per qualsiasi dubbio o informazione aggiuntiva, puoi fare riferimento a <strong>{activity.Referent}</strong>.</p>" +
                                $"<p>Grazie per la collaborazione.<br />" +
                                $"Cordiali saluti<br />" +
                                $"<strong><i>Staff Lacos</i></strong></p>";
                            await sharedService.SendMessage(interventionOperator.Email!, "", "Variazione intervento assegnato", body);
                        }
                    }
                }

            }

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

    private async Task UpdateInterventionServiceFees (Activity activity, Intervention intervention)
    {        
        var maintenancePriceList = await mainentancePriceListItemRepository.Query()
            .AsNoTracking()
            .Include(m => m.MaintenancePriceList)
            .Where(m => activity.Address.DistanceKm <= m.LimitKm)
            .OrderBy(m => m.LimitKm)
            .FirstOrDefaultAsync();
        if (maintenancePriceList != null)
        {
            var TimeSpanDuration = CalculateWorkingHoursDifference(intervention.Start, intervention.End);
            intervention.ServiceFee = maintenancePriceList.MaintenancePriceList!.HourlyRate * TimeSpanDuration;
            intervention.ServiceCallFee = maintenancePriceList.ServiceCallFee;
            intervention.TravelFee = (decimal)maintenancePriceList.TravelFee * (decimal)activity.Address.DistanceKm * 2;
            intervention.ExtraFee = activity.Address.IsInsideAreaC == true ? maintenancePriceList.ExtraFee : 0;
        }
        
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
            .Include(x => x.Job)
            .Include(x => x.Type)
            .Include(x => x.Interventions)
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
            throw new NotFoundException($"Attività con Id {activity.Id} non trovata.");
        }

        var newStatus = !activity.Interventions.Any()
                    ? ActivityStatus.Pending
                    : activity.Interventions.Any(i => i.Status == InterventionStatus.Scheduled)
                        ? ActivityStatus.Ready
                        : ActivityStatus.Completed;

        if (activity.Status != newStatus) {
            logger.LogWarning($"[{activity.JobId}]Commessa {activity.Job.Number.ToString("000")}/{activity.Job.Year}: " +
                $"modifica interventi: attività {activity.RowNumber}/{activity.Type!.Name} cambio stato '{activity.Status}' -> '{newStatus}: " +
                $"impostazione evasione dipendenze");
            activity.Status = newStatus;
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

        await activityRepository.Update(id, e => e.Status = activity.Status);

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

    public async Task<ReportDto> GenerateReport(long interventionId)
    {
        var parameters = new[] { new Parameter("InterventionId", interventionId) };
        var content = Report("Intervento.trdp", parameters);

        string fileName = "intervento.pdf";

        Intervention intervention = await repository.Query()
            .Where(i => i.Id == interventionId)
            .FirstOrDefaultAsync();

        if (intervention != null)
        {
            fileName = "intervento_" + intervention.Start.Year.ToString() + "_" + intervention.Start.Month.ToString("00") + "_" + intervention.Start.Day.ToString("00") 
                + "_" + intervention.Id.ToString() + ".pdf";
            intervention.ReportFileName = fileName;
            intervention.ReportGeneratedOn = DateTimeOffset.Now;
            repository.Update(intervention);
            await dbContext.SaveChanges();
        }

        return await Task.FromResult(new ReportDto(content, fileName));
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

    public IQueryable<InterventionSingleProductReadModel> QuerySingleProduct(long activityId, string product)
    {
        var query = repository.Query()
            .Where(e => e.ActivityId == activityId)
            .SelectMany(e => e.Products, (i, p) => new { intervention = i, product = p })
            .Where(p => p.product.ActivityProduct.Product.Code == product || (p.product.ActivityProduct.Product.QrCodePrefix + p.product.ActivityProduct.Product.QrCodeNumber) == product)
            .Select(e => new InterventionSingleProductReadModel()
            {
                Id = e.intervention.Id,
                ActivityId = activityId,
                Status = e.intervention.Status,
                Start = e.intervention.Start,
                End = e.intervention.End,
                Description = e.intervention.Description,
                Operators = e.intervention.Operators.Select(o => o.Name),
                JobId = e.intervention.Activity.JobId,
                InterventionProductId = e.product.Id
            });

        return query;
    }

    // Piano (pseudocodice dettagliato):
    // 1. Metodo pubblico statico `CalculateWorkingHoursDifference(DateTimeOffset start, DateTimeOffset end)`.
    // 2. Se `end <= start` ritorna 0.
    // 3. Se `start.Date == end.Date` ritorna differenza semplice in ore: (end - start).TotalHours.
    // 4. Altrimenti:
    //    a. Convertire start e end in UTC per lavorare su un riferimento unico (startUtc, endUtc).
    //    b. Iterare ogni giorno intero dal giorno di startUtc fino al giorno di endUtc (inclusi).
    //    c. Per ogni giorno costruire due intervalli lavorativi in UTC:
    //       - mattina: 08:00 - 13:00
    //       - pomeriggio: 14:00 - 18:00
    //    d. Per ciascun intervallo calcolare l'overlap con l'intervallo complessivo [startUtc, endUtc].
    //       - overlap = max(0, min(intervalEnd, endUtc) - max(intervalStart, startUtc))
    //       - sommare le ore di overlap al totale (ogni giornata non supera così i 9h).
    //    e. Restituire il totale come double (ore), arrotondato a 2 decimali.
    // 5. Implementare una funzione locale `OverlapHours` per calcolare le ore di sovrapposizione tra due intervalli.
    // Nota: si usano DateTime in UTC per semplicità e robustezza rispetto a differenze di offset/DST.

    /// <summary>
    /// Calcola la differenza in ore tra due date considerando, se le date sono diverse,
    /// un massimo di 9 ore per giornata lavorativa (08:00-13:00 e 14:00-18:00).
    /// Se le date sono uguali, viene usata la differenza reale in ore senza vincoli sugli orari lavorativi.
    /// </summary>
    /// <param name="start">Data/ora di inizio</param>
    /// <param name="end">Data/ora di fine</param>
    /// <returns>Numero di ore totali calcolate secondo le regole descritte</returns>
    public static decimal CalculateWorkingHoursDifference(DateTimeOffset start, DateTimeOffset end)
    {
        if (end <= start)
        {
            return 0;
        }

        // Se stesso giorno: differenza semplice
        if (start.Date == end.Date)
        {
            return (decimal)Math.Round((end - start).TotalHours, 2);
        }

        // Lavoriamo in UTC per avere un riferimento unico e sicuro rispetto agli offset
        DateTime startUtc = start.ToUniversalTime().DateTime;
        DateTime endUtc = end.ToUniversalTime().DateTime;

        decimal totalHours = 0;

        // Itera giorno per giorno
        for (DateTime day = startUtc.Date; day <= endUtc.Date; day = day.AddDays(1))
        {
            DateTime morningStart = new DateTime(day.Year, day.Month, day.Day, 7, 0, 0, DateTimeKind.Utc);
            DateTime morningEnd = new DateTime(day.Year, day.Month, day.Day, 12, 0, 0, DateTimeKind.Utc);
            DateTime afternoonStart = new DateTime(day.Year, day.Month, day.Day, 13, 0, 0, DateTimeKind.Utc);
            DateTime afternoonEnd = new DateTime(day.Year, day.Month, day.Day, 17, 0, 0, DateTimeKind.Utc);

            totalHours += OverlapHours(morningStart, morningEnd, startUtc, endUtc);
            totalHours += OverlapHours(afternoonStart, afternoonEnd, startUtc, endUtc);
        }

        return (decimal)Math.Round(totalHours,2);

        static decimal OverlapHours(DateTime intervalStart, DateTime intervalEnd, DateTime overallStart, DateTime overallEnd)
        {
            DateTime s = intervalStart > overallStart ? intervalStart : overallStart;
            DateTime e = intervalEnd < overallEnd ? intervalEnd : overallEnd;
            if (e <= s) return 0;
            return (decimal)Math.Round((e - s).TotalHours,2);
        }
    }
}