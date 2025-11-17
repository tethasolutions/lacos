using AutoMapper;
using Lacos.GestioneCommesse.Application.Docs.DTOs;
using Lacos.GestioneCommesse.Dal;
using Lacos.GestioneCommesse.Domain.Docs;
using Lacos.GestioneCommesse.Domain.Registry;
using Lacos.GestioneCommesse.Framework.Exceptions;
using Lacos.GestioneCommesse.Framework.Extensions;
using Lacos.GestioneCommesse.Framework.Session;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace Lacos.GestioneCommesse.Application.Docs.Services;

public class JobAccountingService : IJobAccountingService
{
    private readonly IMapper mapper;
    private readonly IRepository<JobAccounting> repository;
    private readonly IRepository<AccountingType> accountingTypeRepository;
    private readonly IRepository<Message> messageRepository;
    private readonly IRepository<MessageNotification> notificationRepository;
    private readonly ILacosDbContext dbContext;
    private readonly ILacosSession session;

    public JobAccountingService(
        IMapper mapper,
        IRepository<JobAccounting> repository,
        IRepository<AccountingType> accountingTypeRepository,
        IRepository<Message> messageRepository,
        IRepository<MessageNotification> notificationRepository,
        ILacosDbContext dbContext,
        ILacosSession session
    )
    {
        this.mapper = mapper;
        this.repository = repository;
        this.accountingTypeRepository = accountingTypeRepository;
        this.messageRepository = messageRepository;
        this.notificationRepository = notificationRepository;
        this.dbContext = dbContext;
        this.session = session;
    }

    public IQueryable<JobAccountingReadModel> Query()
    {
        return repository.Query()
            .Include(e => e.Job)
            .ThenInclude(e => e.Customer)
            .Include(e => e.AccountingType)
            .Project<JobAccountingReadModel>(mapper);
    }

    public async Task Create(JobAccountingDto jobAccountingDto)
    {
        var jobAccounting = jobAccountingDto.MapTo<JobAccounting>(mapper);

        await repository.Insert(jobAccounting);

        await dbContext.SaveChanges();

        var operatorId = session.CurrentUser?.OperatorId;

        if (operatorId != null && jobAccountingDto.TargetOperators.Count() > 0)
        {
            var accountingType = await accountingTypeRepository.Get(jobAccounting.AccountingTypeId);

            var messageDto = new MessageDto()
            {
                OperatorId = (long)operatorId,
                Date = DateTimeOffset.Now,
                Note = $"Voce contabile - {accountingType?.Name} - è stata pagata. {(jobAccounting.Note.IsNullOrEmpty() ? $"Note: {jobAccounting.Note}" : "")}",
                JobId = jobAccounting.JobId,
                IsFromApp = false
            };

            var message = messageDto.MapTo<Message>(mapper);
            await messageRepository.Insert(message);
            await dbContext.SaveChanges();

            foreach (long @operator in jobAccountingDto.TargetOperators)
            {
                var messageNotificationReply = new MessageNotification
                {
                    MessageId = message.Id,
                    IsRead = false,
                    OperatorId = (long)@operator
                };
                await notificationRepository.Insert(messageNotificationReply);
            }
            await dbContext.SaveChanges();

        }

    }

    public async Task Delete(long id)
    {
        var jobAccounting = await repository.Query()
            .AsSplitQuery()
            .FirstOrDefaultAsync(e => e.Id == id);

        if (jobAccounting == null)
        {
            return;
        }

        repository.Delete(jobAccounting);

        await dbContext.SaveChanges();
    }
    public async Task<JobAccountingDto> Get(long id)
    {
        var jobAccountingDto = await repository.Query()
            .Where(e => e.Id == id)
            .Project<JobAccountingDto>(mapper)
            .FirstOrDefaultAsync();

        if (jobAccountingDto == null)
        {
            throw new NotFoundException($"Voce commessa con Id {id} non trovata.");
        }

        return jobAccountingDto;
    }

    public async Task<JobAccountingDto> Update(JobAccountingDto jobAccountingDto)
    {
        var jobAccounting = await repository.Get(jobAccountingDto.Id);

        if (jobAccounting == null)
        {
            throw new NotFoundException($"Voce commessa con Id {jobAccountingDto.Id} non trovata.");
        }

        jobAccounting = jobAccountingDto.MapTo(jobAccounting, mapper);
        repository.Update(jobAccounting);
        await dbContext.SaveChanges();

        var operatorId = session.CurrentUser?.OperatorId;

        if (operatorId != null && jobAccountingDto.TargetOperators != null && jobAccountingDto.TargetOperators.Count() > 0)
        {
            var accountingType = await accountingTypeRepository.Get(jobAccounting.AccountingTypeId);

            var messageDto = new MessageDto()
            {
                OperatorId = (long)operatorId,
                Date = DateTimeOffset.Now,
                Note = $"Voce contabile - {accountingType?.Name} - è stata pagata. {(jobAccounting.Note.IsNullOrEmpty() ? $"Note: {jobAccounting.Note}" : "")}",
                JobId = jobAccounting.JobId,
                IsFromApp = false
            };

            var message = messageDto.MapTo<Message>(mapper);
            await messageRepository.Insert(message);
            await dbContext.SaveChanges();

            foreach (long @operator in jobAccountingDto.TargetOperators)
            {
                var messageNotificationReply = new MessageNotification
                {
                    MessageId = message.Id,
                    IsRead = false,
                    OperatorId = (long)@operator
                };
                await notificationRepository.Insert(messageNotificationReply);
            }
            await dbContext.SaveChanges();

        }

        return await Get(jobAccounting.Id);
    }
}