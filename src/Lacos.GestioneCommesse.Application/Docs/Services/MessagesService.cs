﻿using AutoMapper;
using Lacos.GestioneCommesse.Application.Docs.DTOs;
using Lacos.GestioneCommesse.Dal;
using Lacos.GestioneCommesse.Dal.Migrations;
using Lacos.GestioneCommesse.Domain.Docs;
using Lacos.GestioneCommesse.Domain.Registry;
using Lacos.GestioneCommesse.Framework.Exceptions;
using Lacos.GestioneCommesse.Framework.Extensions;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Linq.Expressions;

namespace Lacos.GestioneCommesse.Application.Docs.Services;

public class MessagesService : IMessagesService
{
    private readonly IMapper mapper;
    private readonly IRepository<Message> repository;
    private readonly IRepository<MessageNotification> notificationRepository;
    private readonly IRepository<Operator> operatorRepository;
    private readonly ILacosDbContext dbContext;

    public MessagesService(
        IMapper mapper,
        IRepository<Message> repository,
        IRepository<MessageNotification> notificationRepository,
        IRepository<Operator> operatorRepository,
        ILacosDbContext dbContext
    )
    {
        this.mapper = mapper;
        this.repository = repository;
        this.notificationRepository = notificationRepository;
        this.operatorRepository = operatorRepository;
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
        await dbContext.SaveChanges();

        return await Get(Message.Id);
    }

    public async Task CreateNotifications(long messageId)
    {
        Message message = await repository.Query()
            .Where(e => e.Id == messageId)
            .Include(e => e.Job)
            .Include(e => e.Ticket)
            .Include(e => e.Activity)
            .Include(e => e.PurchaseOrder)
            .FirstOrDefaultAsync();

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

        await dbContext.SaveChanges();

        return;
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
        var MessageNotification = await notificationRepository.Query()
            .FirstOrDefaultAsync(e => e.MessageId == messageId && e.OperatorId == operatorId);

        if (MessageNotification == null)
        {
            throw new NotFoundException($"Commento con Id {messageId} non trovato.");
        }

        MessageNotification.IsRead = true;

        notificationRepository.Update(MessageNotification);

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
}