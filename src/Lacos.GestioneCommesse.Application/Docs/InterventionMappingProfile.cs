using AutoMapper;
using Lacos.GestioneCommesse.Application.Docs.DTOs;
using Lacos.GestioneCommesse.Domain.Docs;
using Lacos.GestioneCommesse.Framework.Extensions;

namespace Lacos.GestioneCommesse.Application.Docs;

public class InterventionMappingProfile : Profile
{
    public InterventionMappingProfile()
    {
        CreateMap<Intervention, InterventionReadModel>()
            .MapMember(x => x.Customer, y => y.Activity!.Job!.Customer!.Name)
            .MapMember(x => x.CustomerAddress, y => y.Activity!.CustomerAddress!.StreetAddress + ", " + y.Activity!.CustomerAddress.City + " (" + y.Activity!.CustomerAddress.Province + ")")
            .MapMember(x => x.Operators, y => y.Operators.Select(e => e.Name));

        CreateMap<Intervention, InterventionDto>()
            .MapMember(x => x.JobId, y => y.Activity!.JobId);

        CreateMap<InterventionDto, Intervention>()
            .IgnoreCommonMembers()
            .Ignore(x => x.Status)
            .Ignore(x => x.FinalNotes)
            .Ignore(x => x.ReportFileName)
            .Ignore(x => x.ReportGeneratedOn)
            .Ignore(x => x.CustomerSignatureFileName)
            .Ignore(x => x.Vehicle)
            .Ignore(x => x.Activity)
            .Ignore(x => x.Operators)
            .Ignore(x => x.Notes)
            .Ignore(x => x.Products)
            .Ignore(x => x.Disputes)
            .Ignore(x => x.Tickets)
            .Ignore(x => x.PurchaseOrders);
    }
}