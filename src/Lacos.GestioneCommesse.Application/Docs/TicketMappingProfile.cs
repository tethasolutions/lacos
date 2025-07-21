using System.Linq.Expressions;
using AutoMapper;
using Lacos.GestioneCommesse.Application.Docs.DTOs;
using Lacos.GestioneCommesse.Dal;
using Lacos.GestioneCommesse.Domain.Docs;
using Lacos.GestioneCommesse.Framework.Extensions;

namespace Lacos.GestioneCommesse.Application.Docs;

public class TicketMappingProfile : Profile
{
    public TicketMappingProfile()
    {
        CreateMap<Ticket, TicketReadModel>()
            .MapMember(x => x.Code, y => CustomDbFunctions.FormatCode(y.Number, y.Year, 3))
            .MapMember(x => x.Date, y => y.TicketDate)
            .MapMember(x => x.CustomerName, y => y.Customer!.Name)
            .MapMember(x => x.CustomerEmail, y => (y.Customer!.Email != null ? y.Customer!.Email : y.Customer!.ContactEmail))
            .MapMember(x => x.OperatorName, y => y.Operator!.Name)
            .MapMember(x => x.UnreadMessages, y => y.Messages.SelectMany(e => e.MessageNotifications).Count(e => !e.IsRead))
            .MapMember(x => x.HasInterventions, y => (y.Activity == null ? false : y.Activity.Interventions.Any()));

        CreateMap<TicketDto, Ticket>()
            .IgnoreCommonMembers()
            .Ignore(x => x.Customer)
            .Ignore(x => x.Job)
            .Ignore(x => x.Address)
            .Ignore(x => x.Activity)
            .Ignore(x => x.PurchaseOrder)
            .Ignore(x => x.Pictures)
            .Ignore(x => x.IsNew)
            .Ignore(x => x.Operator)
            .Ignore(x => x.Messages)
            .Ignore(x => x.ReportFileName)
            .Ignore(x => x.ReportGeneratedOn)
            .Ignore(x => x.CustomerSignatureName)
            .Ignore(x => x.CustomerSignatureFileName)
            .MapMember(x => x.TicketDate, (x, y) => y.IsTransient() ? x.Date : y.TicketDate)
            .MapMember(x => x.CustomerId, (x, y) => y.IsTransient() ? x.CustomerId : y.CustomerId)
            .AfterMap(AfterMap);

        CreateMap<Ticket, TicketDto>()
            .MapMember(x => x.Date, y => y.TicketDate)
            .MapMember(x => x.CustomerName, y => y.Customer!.Name);

        CreateMap<TicketPicture, TicketAttachmentReadModel>();
        CreateMap<TicketAttachmentReadModel, TicketPicture>()
           .Ignore(x => x.Ticket)
           .Ignore(x => x.TicketId)
           .IgnoreCommonMembers();

        CreateMap<TicketPicture, TicketAttachmentDto>();
        CreateMap<TicketAttachmentDto, TicketPicture>()
           .Ignore(x => x.Ticket)
           .IgnoreCommonMembers();
    }
    private static void AfterMap(TicketDto ticketDto, Ticket ticket, ResolutionContext context)
    {
        ticketDto.Pictures.Merge(ticket.Pictures, (itemDto, item) => itemDto.Id == item.Id, (_, item) => item.TicketId = ticket.Id, context);
        ticketDto.Messages.Merge(ticket.Messages, (itemDto, item) => itemDto.Id == item.Id, (_, item) => item.TicketId = ticket.Id, context);
    }
}
