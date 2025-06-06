﻿using AutoMapper;
using Lacos.GestioneCommesse.Application.Docs.DTOs;
using Lacos.GestioneCommesse.Dal;
using Lacos.GestioneCommesse.Domain.Docs;
using Lacos.GestioneCommesse.Framework.Exceptions;
using Lacos.GestioneCommesse.Framework.Extensions;
using Lacos.GestioneCommesse.Framework.Session;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Linq.Expressions;
using Telerik.Reporting.Processing;
using Telerik.Reporting;
using Parameter = Telerik.Reporting.Parameter;
using Microsoft.Extensions.Logging;

namespace Lacos.GestioneCommesse.Application.Docs.Services;

public class TicketsService : ITicketsService
{
    private readonly IMapper mapper;
    private readonly IRepository<Ticket> repository;
    private readonly IRepository<TicketPicture> ticketAttachmentRepository;
    private readonly IRepository<Job> jobRepository;
    private readonly ILacosSession session;
    private readonly ILacosDbContext dbContext;
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

    public TicketsService(
        IMapper mapper,
        IRepository<Ticket> repository,
        IRepository<TicketPicture> ticketAttachmentRepository,
        IRepository<Job> jobRepository,
        ILacosSession session,
        ILacosDbContext dbContext,
        ILogger<ActivitiesService> logger
    )
    {
        this.mapper = mapper;
        this.repository = repository;
        this.ticketAttachmentRepository = ticketAttachmentRepository;
        this.jobRepository = jobRepository;
        this.session = session;
        this.dbContext = dbContext;
        this.logger = logger;
    }

    public IQueryable<TicketReadModel> Query()
    {
        return repository.Query()
            .Project<TicketReadModel>(mapper);
    }

    public async Task<TicketDto> Get(long id)
    {
        var TicketDto = await dbContext.ExecuteWithDisabledQueryFilters(async () => await repository.Query()
            .Where(e => e.Id == id)
            .Project<TicketDto>(mapper)
            .FirstOrDefaultAsync(), QueryFilter.OperatorEntity);

        //TicketDto.Messages = TicketDto.Messages.Where(m => m.OperatorId == session.CurrentUser.OperatorId ||
        //    m.TargetOperatorsId.Contains(session.CurrentUser.OperatorId.ToString()));

        if (TicketDto == null)
        {
            throw new NotFoundException($"Ticket con Id {id} non trovato.");
        }

        return TicketDto;
    }

    public async Task<TicketDto> Create(TicketDto TicketDto)
    {
        var Ticket = TicketDto.MapTo<Ticket>(mapper);
        var number = await GetNextNumber(Ticket.TicketDate.Year);

        Ticket.SetCode(Ticket.TicketDate.Year, number);

        Ticket.IsNew= true;

        await repository.Insert(Ticket);

        foreach (var file in Ticket.Pictures)
        {
            var ticketAttachment = file.MapTo<TicketPicture>(mapper);
            ticketAttachment.TicketId = Ticket.Id;
            ticketAttachment.Ticket = Ticket;
            ticketAttachment.Description = file.Description;
            ticketAttachment.FileName = file.FileName;

            await ticketAttachmentRepository.Insert(ticketAttachment);
        }

        await dbContext.SaveChanges();

        return await Get(Ticket.Id);
    }

    public async Task<TicketDto> Update(TicketDto TicketDto)
    {
        var Ticket = await repository.Query()
            .AsNoTracking()
            .Where(e => e.Id == TicketDto.Id)
            .Include(e => e.Activity)
            .Include(e => e.Pictures)
            .Include(e => e.Messages)
            //.Include(x => x.Messages.Where(m => m.OperatorId == session.CurrentUser.OperatorId ||
            //    m.MessageNotifications.Any(mn => mn.OperatorId == session.CurrentUser.OperatorId)))
            .FirstOrDefaultAsync();

        if (Ticket == null)
        {
            throw new NotFoundException($"Ticket con Id {TicketDto.Id} non trovato.");
        }

        Ticket = TicketDto.MapTo(Ticket, mapper);

        Ticket.IsNew = false;

        if (Ticket.Activity != null && Ticket.JobId == null) Ticket.JobId = Ticket.Activity.JobId;

        if (Ticket.Status == TicketStatus.Resolved)
        {
            if (Ticket.Activity != null)
            {
                var PreviousStatus = Ticket.Activity.Status;
                Ticket.Activity.Status = ActivityStatus.Completed;

            }
        }

        repository.Update(Ticket);

        await dbContext.SaveChanges();

        if (Ticket.Activity != null)
        {
            if (Ticket.JobId != null)
            {
                Job job = await jobRepository.Query()
                    .Where(e => e.Id == Ticket.JobId)
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
                        logger.LogWarning($"[{Ticket.JobId}]Commessa {job.Number.ToString("000")}/{job.Year}: " +
                            $"modifica ticket {Ticket.Number}/{Ticket.Year}: " +
                            $"cambio stato commessa '{PreviousStatus}' -> '{job.Status}' ");
                        jobRepository.Update(job);
                        await dbContext.SaveChanges();
                    }
                }
            }
        }

        return await Get(Ticket.Id);
    }

    public async Task Delete(long id)
    {
        var Ticket = await repository.Query()
            .AsSplitQuery()
            .FirstOrDefaultAsync(e => e.Id == id);

        if (Ticket == null)
        {
            return;
        }

        repository.Delete(Ticket);
        
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

    public async Task<TicketCounterDto> GetTicketsCounters()
    {
        var query = repository.Query()
            .AsNoTracking()
            .Where(e => e.Status == TicketStatus.Opened)
            .GroupBy(e => e.Status)
            .Select(group => new TicketCounterDto
            {
                OpenedTickets = group.Where(e => !e.IsNew).Count(),
                NewTickets = group.Where(e => e.IsNew).Count()
            })
            //.Project<ActivityCounterDto>(mapper)
            .FirstOrDefaultAsync();

        return await query;

    }

    // --------------------------------------------------------------------------------------------------------------
    public async Task<IEnumerable<TicketAttachmentReadModel>> GetTicketAttachments(long jobId)
    {
        var ticketAttachments = await ticketAttachmentRepository
            .Query()
            .AsNoTracking()
            .Include(x => x.Ticket)
            .Where(x => x.Ticket.Activity.JobId == jobId)
            .OrderBy(x => x.CreatedOn)
            .ToArrayAsync();

        return ticketAttachments.MapTo<IEnumerable<TicketAttachmentReadModel>>(mapper);
    }

    public async Task<TicketAttachmentReadModel> GetTicketAttachmentDetail(long attachmentId)
    {
        var ticketAttachment = await ticketAttachmentRepository
            .Query()
            .AsNoTracking()
            .Where(x => x.Id == attachmentId)
            .SingleOrDefaultAsync();

        return ticketAttachment.MapTo<TicketAttachmentReadModel>(mapper);
    }

    public async Task<TicketAttachmentReadModel> DownloadTicketAttachment(string filename)
    {
        var ticketAttachment = await ticketAttachmentRepository
            .Query()
            .AsNoTracking()
            .Where(x => x.FileName == filename)
            .SingleOrDefaultAsync();

        return ticketAttachment.MapTo<TicketAttachmentReadModel>(mapper);
    }

    public async Task<TicketAttachmentDto> UpdateTicketAttachment(long id, TicketAttachmentDto attachmentDto)
    {
        var attachment = await ticketAttachmentRepository.Get(id);

        if (attachment == null)
        {
            throw new NotFoundException("Errore allegato");
        }
        attachmentDto.MapTo(attachment, mapper);

        ticketAttachmentRepository.Update(attachment);

        await dbContext.SaveChanges();

        return attachment.MapTo<TicketAttachmentDto>(mapper);
    }

    public async Task<TicketAttachmentDto> CreateTicketAttachment(TicketAttachmentDto attachmentDto)
    {
        var attachment = attachmentDto.MapTo<TicketPicture>(mapper);

        await ticketAttachmentRepository.Insert(attachment);

        await dbContext.SaveChanges();

        return attachment.MapTo<TicketAttachmentDto>(mapper);
    }
    //------------------------------------------------------------------------------------------------------------

    public async Task<ReportDto> GenerateReport(long ticketId)
    {
        var parameters = new[] { new Parameter("TicketId", ticketId) };
        var content = Report("Ticket.trdp", parameters);

        string fileName = "ticket.pdf";

        Ticket ticket = await repository.Query()
            .Where(i => i.Id == ticketId)
            .FirstOrDefaultAsync();

        if (ticket != null)
        {
            fileName = "ticket_" + ticket.TicketDate.Year.ToString() + "_" + ticket.TicketDate.Month.ToString("00") + "_" + ticket.TicketDate.Day.ToString("00")
                + "_" + ticket.Id.ToString() + ".pdf";
            ticket.ReportFileName = fileName;
            ticket.ReportGeneratedOn = DateTimeOffset.Now;
            repository.Update(ticket);
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
}