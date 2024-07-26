using AutoMapper;
using AutoMapper.QueryableExtensions;
using Azure;
using Lacos.GestioneCommesse.Application.Docs.DTOs;
using Lacos.GestioneCommesse.Application.Operators.DTOs;
using Lacos.GestioneCommesse.Dal;
using Lacos.GestioneCommesse.Dal.Migrations;
using Lacos.GestioneCommesse.Domain.Docs;
using Lacos.GestioneCommesse.Domain.Registry;
using Lacos.GestioneCommesse.Framework.Exceptions;
using Lacos.GestioneCommesse.Framework.Extensions;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Linq.Expressions;
using Westcar.WebApplication.Dal;

namespace Lacos.GestioneCommesse.Application.Docs.Services;

public class MessagesService : IMessagesService
{
    private readonly IMapper mapper;
    private readonly IRepository<Message> repository;
    private readonly IRepository<MessageNotification> notificationRepository;
    private readonly IRepository<Operator> operatorRepository;
    private readonly IRepository<Domain.Docs.Activity> activityRepository;
    private readonly IViewRepository<MessagesList> viewRepository;
    private readonly ILacosDbContext dbContext;

    public MessagesService(
        IMapper mapper,
        IRepository<Message> repository,
        IRepository<MessageNotification> notificationRepository,
        IRepository<Operator> operatorRepository,
        IRepository<Domain.Docs.Activity> activityRepository,
        IViewRepository<MessagesList> viewRepository,
        ILacosDbContext dbContext
    )
    {
        this.mapper = mapper;
        this.repository = repository;
        this.notificationRepository = notificationRepository;
        this.operatorRepository = operatorRepository;
        this.activityRepository = activityRepository;
        this.viewRepository = viewRepository;
        this.dbContext = dbContext;
    }

    public IQueryable<MessageReadModel> Query()
    {
        return repository.Query()
            .Project<MessageReadModel>(mapper);
    }

    public async Task<MessageDto> Get(long id)
    {
        var MessageDto = await repository.Query()
            .Where(e => e.Id == id)
            .Project<MessageDto>(mapper)
            .FirstOrDefaultAsync();

        if (MessageDto == null)
        {
            throw new NotFoundException($"Commento con Id {id} non trovato.");
        }

        return MessageDto;
    }  

    public async Task<MessageDto> Create(MessageDto MessageDto)
    {
        var Message = MessageDto.MapTo<Message>(mapper);

        await repository.Insert(Message);

        await dbContext.SaveChanges();

        //

        Message message = await repository.Query()
            .Where(e => e.Id == Message.Id)
            .Include(e => e.Job)
            .Include(e => e.Ticket)
            .Include(e => e.Activity)
            .Include(e => e.PurchaseOrder)
            .FirstOrDefaultAsync();

        var AdminOperators = await operatorRepository.Query()
            .Where(e => e.User.Role == Domain.Security.Role.Administrator).ToListAsync();

        //---------ADMIN OPERATORS ------------------------------------------------------------------------
        if (AdminOperators.Any())
        {
            foreach (Operator @operator in AdminOperators)
            {
                if (message.OperatorId != @operator.Id)
                {
                    var MessageNotification = new MessageNotification
                    {
                        MessageId = message.Id,
                        IsRead = false,
                        OperatorId = @operator.Id
                    };
                    await notificationRepository.Insert(MessageNotification);
                }
            }
        }

        //---------JOB-------------------------------------------------------------------------------------
        if (message.JobId != null)
        {
            if (message.Job.ReferentId != null && message.Job.ReferentId != message.OperatorId)
            {
                var MessageNotification = new MessageNotification
                {
                    MessageId = message.Id,
                    IsRead = false,
                    OperatorId = (long)message.Job.ReferentId
                };
                await notificationRepository.Insert(MessageNotification);
            }
        }

        //---------ACTIVITY-------------------------------------------------------------------------------------
        if (message.ActivityId != null)
        {
            if (message.Activity.ReferentId != null && message.Activity.ReferentId != message.OperatorId)
            {
                var MessageNotification = new MessageNotification
                {
                    MessageId = message.Id,
                    IsRead = false,
                    OperatorId = (long)message.Activity.ReferentId
                };
                await notificationRepository.Insert(MessageNotification);
            }

            Domain.Docs.Activity activity = await activityRepository.Query()
                .Where(e => e.Id == message.ActivityId)
                .Include(e => e.Type)
                .ThenInclude(e => e.Operators)
                .FirstOrDefaultAsync();

            if (activity.Type.Operators.Count() > 0)
            {
                foreach (Operator @operator in message.Activity.Type.Operators)
                {
                    if (message.OperatorId != @operator.Id)
                    {
                        var MessageNotification = new MessageNotification
                        {
                            MessageId = message.Id,
                            IsRead = false,
                            OperatorId = @operator.Id
                        };
                        await notificationRepository.Insert(MessageNotification);
                    }
                }
            }
        }

        //---------TICKET-------------------------------------------------------------------------------------
        if (message.TicketId != null)
        {
            if (message.Ticket.OperatorId != null && message.Ticket.OperatorId != message.OperatorId)
            {
                var MessageNotification = new MessageNotification
                {
                    MessageId = message.Id,
                    IsRead = false,
                    OperatorId = (long)message.Ticket.OperatorId
                };
                await notificationRepository.Insert(MessageNotification);
            }
        }

        //---------PURCHASE ORDER-------------------------------------------------------------------------------------
        if (message.PurchaseOrderId != null)
        {
            if (message.PurchaseOrder.OperatorId != null && message.PurchaseOrder.OperatorId != message.OperatorId)
            {
                var MessageNotification = new MessageNotification
                {
                    MessageId = message.Id,
                    IsRead = false,
                    OperatorId = (long)message.PurchaseOrder.OperatorId
                };
                await notificationRepository.Insert(MessageNotification);
            }
        }
        await dbContext.SaveChanges();

        return await Get(Message.Id);
    }

    public async Task<MessageDto> CreateReply(MessageDto MessageDto, string targetOperators)
    {
        long senderMessageId = MessageDto.Id;
        
        var Message = MessageDto.MapTo<Message>(mapper);
        Message.Id = 0;
        await repository.Insert(Message);
        await dbContext.SaveChanges();

        Message senderMessage = await repository.Get(senderMessageId);

        if (senderMessage != null)
        {
            foreach (long @operator in targetOperators.Split(",").Select(long.Parse).ToList())
            {
                if (Message.OperatorId != @operator)
                {
                    var messageNotificationReply = new MessageNotification
                    {
                        MessageId = Message.Id,
                        IsRead = false,
                        OperatorId = (long)@operator
                    };
                    await notificationRepository.Insert(messageNotificationReply);
                }
            }

            await dbContext.SaveChanges();
        }

        return await Get(Message.Id);
    }

    public async Task<MessageDto> Update(MessageDto MessageDto)
    {
        var Message = await repository
            .Query()
            .Where(x => x.Id == MessageDto.Id)
            .SingleOrDefaultAsync();

        if (Message == null)
        {
            throw new NotFoundException($"Commento con Id {MessageDto.Id} non trovato.");
        }

        Message = MessageDto.MapTo(Message, mapper);

        repository.Update(Message);

        await dbContext.SaveChanges();

        return await Get(Message.Id);
    }

    public async Task Delete(long id)
    {
        var Message = await repository.Query()
            .AsSplitQuery()
            .Include(x => x.MessageNotifications)
            .FirstOrDefaultAsync(e => e.Id == id);

        if (Message == null)
        {
            return;
        }

        repository.Delete(Message);
        
        await dbContext.SaveChanges();
    }

    public async Task ReadMessage(long messageId, long operatorId)
    {
        var MessageNotifications = await notificationRepository.Query()
            .Where(e => e.MessageId == messageId && e.OperatorId == operatorId && e.IsRead == false)
            .ToListAsync();

        if (MessageNotifications == null)
        {
            throw new NotFoundException($"Commento con Id {messageId} non trovato.");
        }

        foreach (var Notification in MessageNotifications)
        {
            Notification.IsRead = true;
            notificationRepository.Update(Notification);
        }

        await dbContext.SaveChanges();

        return;
    }

    public async Task<IEnumerable<MessageReadModel>> GetMessages(long jobId, long activityId, long ticketId, long purchaseOrderId)
    {
        if (jobId != null)
        {
            var MessageDto = await repository.Query()
                .Where(e => e.JobId == jobId)
                .OrderBy(e => e.Date)
            .ToArrayAsync();

        return MessageDto.MapTo<IEnumerable<MessageReadModel>>(mapper);
        }

        if (activityId != null)
        {
            var MessageDto = await repository.Query()
                .Where(e => e.ActivityId == activityId)
                .OrderBy(e => e.Date)
            .ToArrayAsync();
        return MessageDto.MapTo<IEnumerable<MessageReadModel>>(mapper);
        }

        if (ticketId != null)
        {
            var MessageDto = await repository.Query()
                .Where(e => e.TicketId == ticketId)
                .OrderBy(e => e.Date)
            .ToArrayAsync();
        return MessageDto.MapTo<IEnumerable<MessageReadModel>>(mapper);
        }

        if (purchaseOrderId != null)
        {
            var MessageDto = await repository.Query()
                .Where(e => e.PurchaseOrderId == purchaseOrderId)
                .OrderBy(e => e.Date)
            .ToArrayAsync();
        return MessageDto.MapTo<IEnumerable<MessageReadModel>>(mapper);
        }

        throw new NotFoundException($"Messaggi non trovati."); ;
    }

    public IQueryable<MessagesListReadModel> GetMessagesList(long operatorId)
    {
        var Messages = viewRepository.Query()
            .Where(e => e.SenderOperatorId == operatorId || e.TargetOperatorId == operatorId);

        return Messages
            .Project<MessagesListReadModel>(mapper)
            .Distinct();
    }

    public async Task<int> GetUnreadCounter(long operatorId)
    {
        int counter = await viewRepository.Query()
            .Where(x => x.TargetOperatorId == operatorId && x.IsRead == false)
            .CountAsync();

        return counter;
    }

    public async Task<IEnumerable<long>> GetReplyTargetOperators(long messageId, bool replyAll)
    {
        if (replyAll) {
            var operators = await dbContext.ExecuteWithDisabledQueryFilters(async () => await notificationRepository.Query()
                .Where(x => x.MessageId == messageId)
                .Select(x => x.OperatorId)
                .Distinct()
                .ToListAsync(), QueryFilter.OperatorEntity);

            var senderOperator = await repository.Query()
                .Where(x => x.Id == messageId)
                .Select(x => x.OperatorId)
                .FirstOrDefaultAsync();

            operators.Add(senderOperator);

            return operators.MapTo<IEnumerable<long>>(mapper);
        }
        else
        {
            var operators = await repository.Query()
                .Where(x => x.Id == messageId)
                .Select(x => x.OperatorId)
                .ToListAsync();

            return operators.MapTo<IEnumerable<long>>(mapper);
        }
    }
}