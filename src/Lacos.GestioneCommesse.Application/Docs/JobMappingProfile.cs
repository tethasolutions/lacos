using System.Linq.Expressions;
using AutoMapper;
using Lacos.GestioneCommesse.Application.Docs.DTOs;
using Lacos.GestioneCommesse.Dal;
using Lacos.GestioneCommesse.Domain.Docs;
using Lacos.GestioneCommesse.Framework.Extensions;

namespace Lacos.GestioneCommesse.Application.Docs;

public class JobMappingProfile : Profile
{
    private static readonly Expression<Func<Job, JobStatusDto>> StatusExpression = j =>
    j.Activities
    .Any() &&
    j.Activities.All(a => a.Status == ActivityStatus.Completed)
    ?
        JobStatusDto.Completed
    : !j.Activities
            .SelectMany(a => a.Interventions)
            .Any()
            ? JobStatusDto.Pending
            : j.Activities
                .SelectMany(a => a.Interventions)
                .Any(i => i.Status == InterventionStatus.Scheduled)
                ? JobStatusDto.InProgress
                : JobStatusDto.Completed;

    public JobMappingProfile()
    {
        CreateMap<Job, JobReadModel>()
            .MapMember(x => x.Address, y => y.Address != null ? y.Address.StreetAddress + ", " + y.Address.City + " (" + y.Address.Province + ")" : "")
            .MapMember(x => x.Code, y => CustomDbFunctions.FormatCode(y.Number, y.Year, 3))
            .MapMember(x => x.Date, y => y.JobDate)
            .MapMember(x => x.Customer, y => y.Customer!.Name)
            .MapMember(x => x.Status, StatusExpression)
            .MapMember(x => x.CanBeRemoved, y =>
                y.Activities
                    .SelectMany(a => a.Interventions)
                    .All(i => i.Status == InterventionStatus.Scheduled)
            );

        CreateMap<JobDto, Job>()
            .IgnoreCommonMembers()
            .Ignore(x => x.Activities)
            .Ignore(x => x.Customer)
            .Ignore(x => x.Address)
            .Ignore(x => x.Number)
            .Ignore(x => x.Year)
            .Ignore(x => x.PurchaseOrders)
            .Ignore(x => x.Tickets)
            .Ignore(x => x.IsInternalJob)
            .MapMember(x => x.JobDate, (x, y) => y.IsTransient() ? x.Date : y.JobDate)
            .MapMember(x => x.CustomerId, (x, y) => y.IsTransient() ? x.CustomerId : y.CustomerId);

        CreateMap<Job, JobDto>()
            .MapMember(x => x.Date, y => y.JobDate)
            .MapMember(x => x.Status, StatusExpression);
    }
}