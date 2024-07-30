using AutoMapper;
using Lacos.GestioneCommesse.Application.Docs.DTOs;
using Lacos.GestioneCommesse.Application.Operators.DTOs;
using Lacos.GestioneCommesse.Domain.Docs;
using Lacos.GestioneCommesse.Framework.Exceptions;
using Lacos.GestioneCommesse.Framework.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Lacos.GestioneCommesse.Application.Docs.Services;

public interface IMessagesService
{
    IQueryable<MessageReadModel> Query();
    Task<MessageDto> Get(long id);
    Task<IEnumerable<MessageReadModel>> GetMessages(long jobId, long activityId, long ticketId, long purchaseOrderId);
    Task<MessageDto> Create(MessageDto jobDto);
    Task<MessageDto> CreateReply(MessageDto jobDto, string targetOperators);
    Task<MessageDto> Update(MessageDto jobDto);
    Task Delete(long id);
    Task ReadMessage(long messageId, long operatorId);
    IQueryable<MessagesListReadModel> GetMessagesList(long operatorId);
    Task<int> GetUnreadCounter(long operatorId);
    Task<IEnumerable<long>> GetReplyTargetOperators(long messageId, bool replyAll);
    Task<IEnumerable<long>> GetElementTargetOperators(long senderOperatorId, long elementId, string elementType);
}