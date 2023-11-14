using AutoMapper;
using Lacos.GestioneCommesse.Application.Docs.DTOs;
using Lacos.GestioneCommesse.Domain.Docs;
using Lacos.GestioneCommesse.Framework.Extensions;

namespace Lacos.GestioneCommesse.Application.Docs;

public class ActivityMappingProfile : Profile
{
    public ActivityMappingProfile()
    {
        CreateMap<Activity, ActivityReadModel>()
            .MapMember(x => x.Address, y => y.Address != null ? (y.Address.Description != "" ? y.Address.Description + " - " : "") + y.Address.StreetAddress + ", " + y.Address.City + " (" + y.Address.Province + ")" : "")
            .MapMember(x => x.Type, y => y.Type!.Name)
            .MapMember(x => x.Source, y =>
                y.SourceTicket == null
                    ? y.SourcePurchaseOrder == null
                        ? null
                        : y.SourcePurchaseOrder.Description
                    : y.SourceTicket.Description
            )
            .MapMember(x => x.CanBeRemoved, y =>
                y.Interventions
                    .All(i => i.Status == InterventionStatus.Scheduled)
            )
            .MapMember(x => x.Number, y => y.RowNumber)
            .MapMember(x => x.JobCode, y => y.Job!.Year.ToString() + "/" + y.Job.Number.ToString())
            .MapMember(x => x.JobHasHighPriority, y => y.Job!.HasHighPriority)
            .MapMember(x => x.Customer, y => y.Job!.Customer!.Name)
            .MapMember(x => x.ActivityColor, y => y.Type!.ColorHex);

        CreateMap<ActivityDto, Activity>()
            .IgnoreCommonMembers()
            .Ignore(x => x.RowNumber)
            .Ignore(x => x.Address)
            .MapMember(x => x.JobId, (x, y) => y.IsTransient() ? x.JobId : y.JobId)
            .Ignore(x => x.Job)
            .MapMember(x => x.TypeId, (x, y) => y.IsTransient() ? x.TypeId : y.TypeId)
            .Ignore(x => x.Type)
            .Ignore(x => x.SourceTicketId)
            .Ignore(x => x.SourceTicket)
            .Ignore(x => x.SourcePuchaseOrderId)
            .Ignore(x => x.SourcePurchaseOrder)
            .Ignore(x => x.Interventions)
            .Ignore(x => x.ActivityProducts);

        CreateMap<Activity, ActivityDto>()
            .MapMember(x => x.Number, y => y.RowNumber);

        CreateMap<Activity, ActivityDetailDto>()
            .MapMember(x => x.Number, y => y.RowNumber)
            .MapMember(x => x.Job, y => y.Job!.Description)
            .MapMember(x => x.CustomerId, y => y.Job!.CustomerId)
            .MapMember(x => x.Customer, y => y.Job!.Customer!.Name)
            .MapMember(x => x.Address, y => y.Address != null ? y.Address.StreetAddress + ", " + y.Address.City + " (" + y.Address.Province + ")" : "")
            .MapMember(x => x.Type, y => y.Type!.Name)
            .MapMember(x => x.Source, y =>
                y.SourceTicket == null
                    ? y.SourcePurchaseOrder == null
                        ? null
                        : y.SourcePurchaseOrder.Description
                    : y.SourceTicket.Description
            );

    }
}