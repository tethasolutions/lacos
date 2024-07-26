using AutoMapper;
using Azure.Core;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Lacos.GestioneCommesse.Application.Docs.DTOs;
using Lacos.GestioneCommesse.Application.Docs.Services;
using Lacos.GestioneCommesse.Framework.Configuration;
using Lacos.GestioneCommesse.Framework.IO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Configuration;

namespace Lacos.GestioneCommesse.WebApi.Controllers;

public class MessagesController : LacosApiController
{
    private readonly IMessagesService service;
    private readonly ILacosConfiguration configuration;
    private readonly IMimeTypeProvider mimeTypeProvider;

    public MessagesController(IMessagesService service, ILacosConfiguration configuration, IMimeTypeProvider mimeTypeProvider)
    {
        this.service = service;
        this.configuration = configuration;
        this.mimeTypeProvider = mimeTypeProvider;
    }

    [HttpGet("read")]
    public Task<DataSourceResult> Read(DataSourceRequest request)
    {
        return service.Query()
            .ToDataSourceResultAsync(request);
    }

    [HttpGet("{id}")]
    public Task<MessageDto> Get(long id)
    {
        return service.Get(id);
    }

    [HttpGet("get-messages/{jobId}/{activityId}/{ticketId}/{purchaseOrderId}")]
    public async Task<List<MessageReadModel>> GetMessages(long jobId, long activityId, long ticketId, long purchaseOrderId)
    {
        List<MessageReadModel> messages = (await service.GetMessages(jobId, activityId, ticketId, purchaseOrderId)).ToList();
        return messages;
    }

    [HttpPost]
    public Task<MessageDto> Create(MessageDto messageDto)
    {
        return service.Create(messageDto);
    }

    [HttpPut("create-reply/{replyAll}")]
    public Task<MessageDto> CreateReply(MessageDto messageDto, bool replyAll)
    {
        return service.CreateReply(messageDto, replyAll);
    }

    [HttpPut("{id}")]
    public Task<MessageDto> Update(long id, MessageDto messageDto)
    {
        messageDto.Id = id;

        return service.Update(messageDto);
    }

    [HttpDelete("{id}")]
    public Task Delete(long id)
    {
        return service.Delete(id);
    }

    [HttpGet("mark-as-read/{messageId}/{operatorId}")]
    public Task MarkAsRead(long messageId, long operatorId)
    {
        return service.ReadMessage(messageId, operatorId);
    }


    [HttpGet("{operatorId}/get-messageslist")]
    public Task<DataSourceResult> GetMessagesList([DataSourceRequest] DataSourceRequest request, long operatorId)
    {
        return service.GetMessagesList(operatorId)
            .ToDataSourceResultAsync(request);
    }

    [HttpGet("unread-counter/{operatorId}")]
    public async Task<int> GetUnreadCounter(long operatorId)
    {
        return await service.GetUnreadCounter(operatorId);
    }

}