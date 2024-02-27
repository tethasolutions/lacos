using AutoMapper;
using Lacos.GestioneCommesse.Application.Docs.DTOs;
using Lacos.GestioneCommesse.Dal;
using Lacos.GestioneCommesse.Domain.Docs;
using Lacos.GestioneCommesse.Framework.Exceptions;
using Lacos.GestioneCommesse.Framework.Extensions;
using Lacos.GestioneCommesse.Framework.Session;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Linq.Expressions;

namespace Lacos.GestioneCommesse.Application.Docs.Services;

public class TicketsService : ITicketsService
{
    private readonly IMapper mapper;
    private readonly IRepository<Ticket> repository;
    private readonly IRepository<TicketPicture> ticketAttachmentRepository;
    private readonly IRepository<Job> jobRepository;
    private readonly ILacosDbContext dbContext;

    private static readonly Expression<Func<Job, JobStatus>> StatusExpression = j =>
    j.Activities
    .Any() &&
    j.Activities.All(a => a.Status == ActivityStatus.Completed)
    ?
        JobStatus.Completed
    : j.Activities
            .Any(a => a.Status == ActivityStatus.InProgress)
            ? JobStatus.InProgress
            : j.Activities
                .Any(a => a.Status == ActivityStatus.Pending)
                ? JobStatus.Pending
                : j.Status;

    public TicketsService(
        IMapper mapper,
        IRepository<Ticket> repository,
        IRepository<TicketPicture> ticketAttachmentRepository,
        IRepository<Job> jobRepository,
        ILacosDbContext dbContext
    )
    {
        this.mapper = mapper;
        this.repository = repository;
        this.ticketAttachmentRepository = ticketAttachmentRepository;
        this.jobRepository = jobRepository;
        this.dbContext = dbContext;
    }

    public IQueryable<TicketReadModel> Query()
    {
        return repository.Query()
            .Project<TicketReadModel>(mapper);
    }

    public async Task<TicketDto> Get(long id)
    {
        var TicketDto = await repository.Query()
            .Where(e => e.Id == id)
            .Project<TicketDto>(mapper)
            .FirstOrDefaultAsync();

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
            .FirstOrDefaultAsync();

        if (Ticket == null)
        {
            throw new NotFoundException($"Ticket con Id {TicketDto.Id} non trovato.");
        }

        Ticket = TicketDto.MapTo(Ticket, mapper);

        Ticket.IsNew = false;

        if (Ticket.Status == TicketStatus.Resolved)
        {
            if (Ticket.Activity != null) Ticket.Activity.Status = ActivityStatus.Completed;
        }

        repository.Update(Ticket);

        await dbContext.SaveChanges();

        if (Ticket.Activity != null)
        {
            if (Ticket.Activity.JobId != null)
            {
                Job job = await jobRepository.Query()
                    .Where(e => e.Id == Ticket.Activity.JobId)
                    .Include(e => e.Activities)
                    .FirstOrDefaultAsync();
                if (job != null)
                {
                    Func<Job, JobStatus> statusDelegate = StatusExpression.Compile();
                    job.Status = statusDelegate(job);
                    jobRepository.Update(job);
                    await dbContext.SaveChanges();
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
}