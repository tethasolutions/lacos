using System.Linq.Expressions;
using AutoMapper;
using Lacos.GestioneCommesse.Application.Docs.DTOs;
using Lacos.GestioneCommesse.Domain.Docs;
using Lacos.GestioneCommesse.Framework.Extensions;

namespace Lacos.GestioneCommesse.Application.Docs;

public class TicketMappingProfile : Profile
{
    public TicketMappingProfile()
    {
        CreateMap<Ticket, TicketReadModel>()
            .MapMember(x => x.Code, y => y.Year.ToString() + "/" + y.Number.ToString())
            .MapMember(x => x.Date, y => y.TicketDate)
            .MapMember(x => x.CustomerName, y => y.Customer.Name)
            .MapMember(x => x.CustomerFullAddress, y => y.CustomerAddress.City + " - " + y.CustomerAddress.StreetAddress);

        CreateMap<TicketDto, Ticket>()
            .IgnoreCommonMembers()
            .Ignore(x => x.Customer)
            .Ignore(x => x.Number)
            .Ignore(x => x.Year)
            .MapMember(x => x.TicketDate, (x, y) => y.IsTransient() ? x.Date : y.TicketDate)
            .MapMember(x => x.CustomerId, (x, y) => y.IsTransient() ? x.CustomerId : y.CustomerId);

        CreateMap<Ticket, TicketDto>()
            .MapMember(x => x.Date, y => y.TicketDate)
            .MapMember(x => x.CustomerName, y => y.Customer.Name)
            .MapMember(x => x.CustomerFullAddress, y => y.CustomerAddress.City + " - " + y.CustomerAddress.StreetAddress);
    }
}