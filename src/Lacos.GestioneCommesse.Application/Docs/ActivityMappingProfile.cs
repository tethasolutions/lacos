using AutoMapper;
using Lacos.GestioneCommesse.Application.Docs.DTOs;
using Lacos.GestioneCommesse.Domain.Docs;
using Lacos.GestioneCommesse.Framework.Extensions;
using System.Linq.Expressions;

namespace Lacos.GestioneCommesse.Application.Docs;

public class ActivityMappingProfile : Profile
{
    private static readonly Expression<Func<Activity, ActivityStatusDto>> StatusExpression = j =>
        !j.Interventions
            .Any()
            ? ActivityStatusDto.Pending
            : j.Interventions
                .Any(i => i.Status == InterventionStatus.Scheduled)
                ? ActivityStatusDto.InProgress
                : ActivityStatusDto.Completed;

    public ActivityMappingProfile()
    {
        CreateMap<Activity, ActivityReadModel>()
            .MapMember(x => x.CustomerAddress, y => y.CustomerAddress!.StreetAddress + ", " + y.CustomerAddress.City + " (" + y.CustomerAddress.Province + ")")
            .MapMember(x => x.Type, y => y.Type!.Name)
            .MapMember(x => x.Source, y =>
                y.SourceTicket == null
                    ? y.SourcePurchaseOrder == null
                        ? null
                        : y.SourcePurchaseOrder.Description
                    : y.SourceTicket.Description
            )
            .MapMember(x => x.Status, StatusExpression)
            .MapMember(x => x.CanBeRemoved, y =>
                y.Interventions
                    .All(i => i.Status == InterventionStatus.Scheduled)
            )
            .MapMember(x => x.Number, y => y.RowNumber);

        CreateMap<ActivityDto, Activity>()
            .IgnoreCommonMembers()
            .Ignore(x => x.RowNumber)
            .MapMember(x => x.CustomerAddressId, (x, y) => y.IsTransient() ? x.CustomerAddressId : y.CustomerAddressId)
            .Ignore(x => x.CustomerAddress)
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
            .MapMember(x => x.Number, y => y.RowNumber)
            .MapMember(x => x.Status, StatusExpression);

        CreateMap<Activity, ActivityDetailDto>()
            .MapMember(x => x.Number, y => y.RowNumber)
            .MapMember(x => x.Job, y => y.Job!.Description)
            .MapMember(x => x.CustomerId, y => y.Job!.CustomerId)
            .MapMember(x => x.Customer, y => y.Job!.Customer!.Name)
            .MapMember(x => x.CustomerAddress, y => y.CustomerAddress!.StreetAddress + ", " + y.CustomerAddress.City + " (" + y.CustomerAddress.Province + ")")
            .MapMember(x => x.Type, y => y.Type!.Name)
            .MapMember(x => x.Source, y =>
                y.SourceTicket == null
                    ? y.SourcePurchaseOrder == null
                        ? null
                        : y.SourcePurchaseOrder.Description
                    : y.SourceTicket.Description
            )
            .MapMember(x => x.Status, StatusExpression);
    }
}