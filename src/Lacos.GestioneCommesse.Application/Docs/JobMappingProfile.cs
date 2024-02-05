﻿using System.Linq.Expressions;
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
            //.MapMember(x => x.Status, StatusExpression)
            .MapMember(x => x.CanBeRemoved, y =>
                y.Activities
                    .SelectMany(a => a.Interventions)
                    .All(i => i.Status == InterventionStatus.Scheduled)
            )
            .MapMember(x => x.HasActivities, y => y.Activities.Any())
            .MapMember(x => x.HasAttachments, y => y.Activities.Where(a => a.Attachments.Any()).Any())
            .MapMember(x => x.ReferentName, y => (y.Referent != null) ? y.Referent.Name : "")
            .MapMember(x => x.HasPurchaseOrders, y => y.PurchaseOrders.Any());

        CreateMap<JobDto, Job>()
            .IgnoreCommonMembers()
            .Ignore(x => x.Activities)
            .Ignore(x => x.Customer)
            .Ignore(x => x.Address)
            .Ignore(x => x.Number)
            .Ignore(x => x.Year)
            .Ignore(x => x.PurchaseOrders)
            .Ignore(x => x.Tickets)
            .Ignore(x => x.Attachments)
            .Ignore(x => x.Referent)
            .Ignore(x => x.IsInternalJob)
            .MapMember(x => x.JobDate, (x, y) => y.IsTransient() ? x.Date : y.JobDate)
            .MapMember(x => x.CustomerId, (x, y) => y.IsTransient() ? x.CustomerId : y.CustomerId)
            .AfterMap(AfterMap);

        CreateMap<Job, JobDto>()
            .MapMember(x => x.Date, y => y.JobDate)
            .MapMember(x => x.Status, StatusExpression);

        CreateMap<JobAttachment, JobAttachmentReadModel>();
        CreateMap<JobAttachmentReadModel, JobAttachment>()
           .Ignore(x => x.Job)
           .Ignore(x => x.JobId)
           .IgnoreCommonMembers();

        CreateMap<JobAttachment, JobAttachmentDto>();
        CreateMap<JobAttachmentDto, JobAttachment>()
           .Ignore(x => x.Job)
           .IgnoreCommonMembers();
    }
    private static void AfterMap(JobDto jobDto, Job job, ResolutionContext context)
    {
        jobDto.Attachments.Merge(job.Attachments, (itemDto, item) => itemDto.Id == item.Id, (_, item) => item.JobId = job.Id, context);
    }
}