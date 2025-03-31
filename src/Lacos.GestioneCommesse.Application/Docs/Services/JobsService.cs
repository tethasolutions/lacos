using AutoMapper;
using Lacos.GestioneCommesse.Application.Docs.DTOs;
using Lacos.GestioneCommesse.Dal;
using Lacos.GestioneCommesse.Domain.Docs;
using Lacos.GestioneCommesse.Domain.Registry;
using Lacos.GestioneCommesse.Framework.Exceptions;
using Lacos.GestioneCommesse.Framework.Extensions;
using Lacos.GestioneCommesse.Framework.Session;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Linq.Expressions;
using Westcar.WebApplication.Dal;

namespace Lacos.GestioneCommesse.Application.Docs.Services;

public class JobsService : IJobsService
{
    private readonly IMapper mapper;
    private readonly IRepository<Job> repository;
    private readonly IRepository<Operator> operatorRepository;
    private readonly IRepository<JobAttachment> jobAttachmentRepository;
    private readonly IViewRepository<JobsProgressStatus> jobsProgressStatusRepository;
    private readonly IRepository<Ticket> ticketRepository;
    private readonly IRepository<Customer> customerRepository;
    private readonly ILacosSession session;
    private readonly ILacosDbContext dbContext;

    public JobsService(
        IMapper mapper,
        IRepository<Job> repository,
        IRepository<Operator> operatorRepository,
        IRepository<JobAttachment> jobAttachmentRepository,
        IViewRepository<JobsProgressStatus> jobsProgressStatusRepository,
        IRepository<Ticket> ticketRepository,
        IRepository<Customer> customerRepository,
        ILacosSession session,
        ILacosDbContext dbContext
    )
    {
        this.mapper = mapper;
        this.repository = repository;
        this.operatorRepository = operatorRepository;
        this.jobAttachmentRepository = jobAttachmentRepository;
        this.jobsProgressStatusRepository = jobsProgressStatusRepository;
        this.ticketRepository = ticketRepository;
        this.customerRepository = customerRepository;
        this.session = session;
        this.dbContext = dbContext;
    }

    public IQueryable<JobReadModel> Query()
    {
        return repository.Query()
            //.Where(e => !e.IsInternalJob)
            .Include(e => e.Referent)
            .Project<JobReadModel>(mapper);
    }

    public IQueryable<JobReadModel> QueryCurrentJobs()
    {
        return repository.Query()
            .Where(e => (e.Year >= DateTime.Now.Year && e.Status == JobStatus.Billed) || 
                (e.Status == JobStatus.Billing || e.Status == JobStatus.Completed))
            .Include(e => e.Referent)
            .Project<JobReadModel>(mapper);
    }

    public IQueryable<JobReadModel> QueryArchivedJobs()
    {
        return repository.Query()
            .Where(e => e.Year < DateTime.Now.Year)
            .Include(e => e.Referent)
            .Project<JobReadModel>(mapper);
    }

    public async Task<JobDto> GetTicketJob(long CustomerId, string TicketCode)
    {
        var jobDto = await repository.Query()
            .Where(e => e.IsInternalJob && e.Year == DateTime.Now.Year && e.CustomerId == CustomerId)
            .Project<JobDto>(mapper)
            .FirstOrDefaultAsync();

        if (jobDto == null)
        {
            var customer = await customerRepository.Get(CustomerId);
            
            jobDto = new JobDto();
            jobDto.CustomerId = CustomerId;
            jobDto.Year = DateTime.Now.Year;
            jobDto.Reference = "Ticket " + TicketCode + ((customer != null) ? " - " + customer.Name : "");
            jobDto.Date = DateTime.Now;
            jobDto.Status = JobStatus.Pending;
            jobDto.Description = " ";
            jobDto.Attachments = Enumerable.Empty<JobAttachmentDto>();
            jobDto.Messages = Enumerable.Empty<MessageReadModel>();

            var job = jobDto.MapTo<Job>(mapper);
            job.IsInternalJob= true;
            var number = await GetNextNumber(job.JobDate.Year);
            job.SetCode(job.JobDate.Year, number);

            await repository.Insert(job);

            await dbContext.SaveChanges();

            var operatorJob = await operatorRepository.Query()
                .Where(e => e.UserId == job.CreatedById)
                .FirstOrDefaultAsync();

            if (operatorJob != null)
            {
                job.ReferentId = operatorJob.Id;
                repository.Update(job);
                await dbContext.SaveChanges();
            }

            return await Get(job.Id);
        }
        else
            return jobDto;
    }

    public async Task<JobDto> Get(long id)
    {
        var jobDto = await dbContext.ExecuteWithDisabledQueryFilters(async () => await repository.Query()
            .Where(e => e.Id == id)
            .Project<JobDto>(mapper)
            .FirstOrDefaultAsync(), QueryFilter.OperatorEntity);

        //jobDto.Messages = jobDto.Messages.Where(m => m.OperatorId == session.CurrentUser.OperatorId ||
        //    m.TargetOperatorsId.Contains(session.CurrentUser.OperatorId.ToString()));

        if (jobDto == null)
        {
            throw new NotFoundException($"Commessa con Id {id} non trovata.");
        }

        return jobDto;
    }  

    public async Task<JobDto> Create(JobDto jobDto)
    {
        var job = jobDto.MapTo<Job>(mapper);
        var number = await GetNextNumber(job.JobDate.Year);

        job.SetCode(job.JobDate.Year, number);
        if (job.Description == null) job.Description = " ";

        var customer = await customerRepository.Query().Include(x => x.Addresses).Where(x => x.Id == job.CustomerId).FirstOrDefaultAsync();
        if (customer != null)
        {
            job.Reference = number.ToString("000") + "/" + job.Year.ToString().Substring(2, 2) + " " + customer.Name;
            if (job.AddressId != null)
            {
                job.Reference += $"({customer.Addresses.Where(a => a.Id == job.AddressId).Select(a => a.JobReference).FirstOrDefault()})";
            }
        }

        await repository.Insert(job);

        foreach (var file in job.Attachments)
        {
            var jobAttachment = file.MapTo<JobAttachment>(mapper);
            jobAttachment.JobId = job.Id;
            jobAttachment.Job = job;
            jobAttachment.DisplayName = file.DisplayName;
            jobAttachment.FileName = file.FileName;

            await jobAttachmentRepository.Insert(jobAttachment);
        }

        await dbContext.SaveChanges();

        return await Get(job.Id);
    }

    public async Task<JobDto> Update(JobDto jobDto)
    {
        var job = await repository
            .Query()
            .Where(x => x.Id == jobDto.Id)
            .Include(x => x.Attachments)
            .Include(x => x.Messages)
            //.Include(x => x.Messages.Where(m => m.OperatorId == session.CurrentUser.OperatorId ||
            //    m.MessageNotifications.Any(mn => mn.OperatorId == session.CurrentUser.OperatorId)))
            .SingleOrDefaultAsync();

        if (job == null)
        {
            throw new NotFoundException($"Commessa con Id {jobDto.Id} non trovata.");
        }

        job = jobDto.MapTo(job, mapper);

        repository.Update(job);

        //se il job viene impostato come fatturata allora chiudo il relativo ticket se presente
        if (job.Status == JobStatus.Billed)
        {
            var tickets = await ticketRepository.Query()
                .Where(e => e.JobId == job.Id || e.Activity.JobId == job.Id)
                .ToListAsync();

            if (tickets != null)
            {
                foreach (var ticket in tickets)
                {
                    ticket.Status = TicketStatus.Closed;
                    ticketRepository.Update(ticket);
                }
            }
        }
                
        await dbContext.SaveChanges();

        return await Get(job.Id);
    }

    public async Task Delete(long id)
    {
        var job = await repository.Query()
            .AsSplitQuery()
            .Include(e => e.Activities)
            .ThenInclude(e => e.Interventions)
            .Include(e => e.Activities)
            .ThenInclude(e => e.ActivityProducts)
            .ThenInclude(e => e.InterventionProducts)
            .ThenInclude(e => e.CheckList)
            .ThenInclude(e => e!.Items)
            .FirstOrDefaultAsync(e => e.Id == id);

        if (job == null)
        {
            return;
        }

        if (job.HasCompletedInterventions())
        {
            throw new LacosException("Non puoi eliminare una commessa con interventi completati.");
        }

        repository.Delete(job);
        
        await dbContext.SaveChanges();
    }

    private async Task<int> GetNextNumber(int year)
    {
        var maxNumber = await repository.Query()
            .Where(e => e.Year == year)
            .Select(e => (int?)e.Number)
            .MaxAsync();

        return (maxNumber ?? 0) + 1;
    }

    public async Task<long> CopyJob(JobCopyDto jobCopyDto)
    {
        var newJob = await repository.Query()
            .AsNoTracking()
            .Where(e => e.Id == jobCopyDto.OriginalId)
            .Include(e => e.Activities)
            .FirstOrDefaultAsync();

        DateTime originalDate = newJob.JobDate.Date;
        newJob.Id = 0;
        newJob.JobDate = jobCopyDto.Date;
        newJob.Year = jobCopyDto.Date.Year;
        newJob.CustomerId = jobCopyDto.CustomerId;
        newJob.AddressId = jobCopyDto.AddressId;
        newJob.Reference = jobCopyDto?.Reference;
        newJob.Description= jobCopyDto?.Description;
        var number = await GetNextNumber(newJob.JobDate.Year);
        newJob.SetCode(newJob.JobDate.Year, number);

        foreach (var activity in newJob.Activities)
        {
            activity.Id = 0;
            if (activity.ExpirationDate!= null)
            {
                var dateDiff = activity.ExpirationDate - originalDate;
                activity.ExpirationDate = newJob.JobDate + dateDiff;
            }
        }

        await repository.Insert(newJob);

        await dbContext.SaveChanges();

        return newJob.Id;
    }

    // --------------------------------------------------------------------------------------------------------------
    public async Task<IEnumerable<JobAttachmentReadModel>> GetJobAttachments(long jobId)
    {
        var jobAttachments = await jobAttachmentRepository
            .Query()
            .AsNoTracking()
            .Where(x => x.JobId == jobId)
            .OrderBy(x => x.CreatedOn)
            .ToArrayAsync();

        return jobAttachments.MapTo<IEnumerable<JobAttachmentReadModel>>(mapper);
    }

    public async Task<JobAttachmentReadModel> GetJobAttachmentDetail(long attachmentId)
    {
        var jobAttachment = await jobAttachmentRepository
            .Query()
            .AsNoTracking()
            .Where(x => x.Id == attachmentId)
            .SingleOrDefaultAsync();

        return jobAttachment.MapTo<JobAttachmentReadModel>(mapper);
    }

    public async Task<JobAttachmentReadModel> DownloadJobAttachment(string filename)
    {
        var jobAttachment = await jobAttachmentRepository
            .Query()
            .AsNoTracking()
            .Where(x => x.FileName == filename)
            .SingleOrDefaultAsync();

        return jobAttachment.MapTo<JobAttachmentReadModel>(mapper);
    }

    public async Task<JobAttachmentDto> UpdateJobAttachment(long id, JobAttachmentDto attachmentDto)
    {
        var attachment = await jobAttachmentRepository.Get(id);

        if (attachment == null)
        {
            throw new NotFoundException("Errore allegato");
        }
        attachmentDto.MapTo(attachment, mapper);

        jobAttachmentRepository.Update(attachment);

        await dbContext.SaveChanges();

        return attachment.MapTo<JobAttachmentDto>(mapper);
    }

    public async Task<JobAttachmentDto> CreateJobAttachment(JobAttachmentDto attachmentDto)
    {
        var attachment = attachmentDto.MapTo<JobAttachment>(mapper);

        await jobAttachmentRepository.Insert(attachment);

        await dbContext.SaveChanges();

        return attachment.MapTo<JobAttachmentDto>(mapper);
    }

    public IQueryable<JobsProgressStatusReadModel> GetJobsProgressStatus()
    {
        var jobsProgressStatus = jobsProgressStatusRepository.Query();

        return jobsProgressStatus
            .Project<JobsProgressStatusReadModel>(mapper);
    }
}