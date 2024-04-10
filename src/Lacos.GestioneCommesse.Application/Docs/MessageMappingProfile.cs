using System.Linq.Expressions;
using AutoMapper;
using Lacos.GestioneCommesse.Application.Docs.DTOs;
using Lacos.GestioneCommesse.Dal;
using Lacos.GestioneCommesse.Domain.Docs;
using Lacos.GestioneCommesse.Framework.Extensions;

namespace Lacos.GestioneCommesse.Application.Docs;

public class MessageMappingProfile : Profile
{
    public MessageMappingProfile()
    {
        CreateMap<Message, MessageDto>();

        CreateMap<MessageDto, Message>()
            .IgnoreCommonMembers()
            .Ignore(x => x.Operator)
            .Ignore(x => x.Activity)
            .Ignore(x => x.Ticket)
            .Ignore(x => x.Job)
            .Ignore(x => x.PurchaseOrder)
            .Ignore(x => x.MessageNotifications);

        CreateMap<Message, MessageReadModel>()
            .MapMember(x => x.OperatorName, y => y.Operator!.Name)
            .MapMember(x => x.ElementCode, y => (y.JobId != null) ? "Commessa " + CustomDbFunctions.FormatCode(y.Job.Number, y.Job.Year, 3) :
                    ((y.ActivityId != null) ? "Attività " + y.Activity.Type.Name :
                    ((y.TicketId != null) ? "Ticket " + CustomDbFunctions.FormatCode(y.Ticket.Number, y.Ticket.Year, 3) :
                    ((y.PurchaseOrderId != null) ? "Ordine " + CustomDbFunctions.FormatCode(y.PurchaseOrder.Number, y.PurchaseOrder.Year, 3) : null))))
            .MapMember(x => x.IsRead, y => (!y.MessageNotifications.Any() || y.MessageNotifications.All(e => e.IsRead)));

        CreateMap<MessageReadModel, Message>()
            .IgnoreCommonMembers()
            .Ignore(x => x.Operator)
            .Ignore(x => x.Activity)
            .Ignore(x => x.Ticket)
            .Ignore(x => x.Job)
            .Ignore(x => x.PurchaseOrder)
            .Ignore(x => x.MessageNotifications);
    }
}