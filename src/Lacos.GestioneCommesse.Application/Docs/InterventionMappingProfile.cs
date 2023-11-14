using AutoMapper;
using Lacos.GestioneCommesse.Application.Docs.DTOs;
using Lacos.GestioneCommesse.Domain.Docs;
using Lacos.GestioneCommesse.Domain.Registry;
using Lacos.GestioneCommesse.Framework.Extensions;

namespace Lacos.GestioneCommesse.Application.Docs;

public class InterventionMappingProfile : Profile
{
    public InterventionMappingProfile()
    {
        CreateMap<Intervention, InterventionReadModel>()
            .MapMember(x => x.Customer, y => y.Activity!.Job!.Customer!.Name)
            .MapMember(x => x.Address, y => y.Activity!.Address!.StreetAddress + ", " + y.Activity!.Address.City + " (" + y.Activity!.Address.Province + ")")
            .MapMember(x => x.ActivityType, y => y.Activity!.Type!.Name)
            .MapMember(x => x.ActivityTypeId, y => y.Activity!.TypeId)
            .MapMember(x => x.ActivityColor, y => y.Activity!.Type!.ColorHex)
            .MapMember(x => x.CanBeRemoved, y => y.Status == InterventionStatus.Scheduled);

        CreateMap<Operator, InterventionOperatorReadModel>();

        CreateMap<Intervention, InterventionDto>()
            .MapMember(x => x.JobId, y => y.Activity!.JobId)
            .MapMember(x => x.Operators, y => y.Operators.Select(e => e.Id))
            .MapMember(x => x.ActivityProducts, y => y.Products.Select(e => e.ActivityProductId));

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
            .Ignore(x => x.Disputes);
    }
}