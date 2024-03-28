using AutoMapper;
using Lacos.GestioneCommesse.Application.Docs.DTOs;
using Lacos.GestioneCommesse.Dal;
using Lacos.GestioneCommesse.Domain.Docs;
using Lacos.GestioneCommesse.Framework.Extensions;

namespace Lacos.GestioneCommesse.Application.Docs;

public class ActivityMappingProfile : Profile
{
    public ActivityMappingProfile()
    {
        CreateMap<Activity, ActivityReadModel>()
           .MapMember(x => x.Address, y =>
                y.Address == null
                    ? ""
                    : (string.IsNullOrEmpty(y.Address.Description) ? "" : y.Address.Description + " - ") + y.Address.StreetAddress + ", " + y.Address.City +
                    " (" + y.Address.Province + ")"
            )
           .MapMember(x => x.Type, y => y.Type!.Name)
           .MapMember(x => x.CanBeRemoved, y =>
                y.Interventions
                   .All(i => i.Status == InterventionStatus.Scheduled)
            )
           .MapMember(x => x.Number, y => y.RowNumber)
           .MapMember(x => x.JobCode, y => CustomDbFunctions.FormatCode(y.Job!.Number, y.Job.Year, 3))
           .MapMember(x => x.JobReference, y => y.Job!.Reference)
           .MapMember(x => x.JobHasHighPriority, y => y.Job!.HasHighPriority)
           .MapMember(x => x.CustomerId, y => y.Job!.CustomerId)
           .MapMember(x => x.Customer, y => y.Job!.Customer == null ? null : y.Job.Customer.Name)
           .MapMember(x => x.ActivityColor, y => y.Type!.ColorHex)
           .MapMember(x => x.LastOperator, y => y.CreatedBy)
           .MapMember(x => x.ReferentName, y => (y.Referent != null) ? y.Referent.Name : "")
           .MapMember(x => x.HasAttachments, y => y.Attachments.Any())
           .MapMember(x => x.IsExpired, y => (y.ExpirationDate != null) ? (y.ExpirationDate < DateTime.Now.Date) : false)
           .MapMember(x => x.IsInternal, y => y.Type!.IsInternal)
           .MapMember(x => x.StatusLabel0, y => y.Type!.StatusLabel0)
           .MapMember(x => x.StatusLabel1, y => y.Type!.StatusLabel1)
           .MapMember(x => x.StatusLabel2, y => y.Type!.StatusLabel2);
         
        CreateMap<ActivityDto, Activity>()
           .IgnoreCommonMembers()
           .Ignore(x => x.RowNumber)
           .Ignore(x => x.Supplier)
           .Ignore(x => x.Address)
           .MapMember(x => x.JobId, (x, y) => y.IsTransient() ? x.JobId : y.JobId)
           .Ignore(x => x.Job)
           .MapMember(x => x.TypeId, (x, y) => y.IsTransient() ? x.TypeId : y.TypeId)
           .Ignore(x => x.Type)
           .Ignore(x => x.Interventions)
           .Ignore(x => x.ActivityProducts)
           .Ignore(x => x.Referent)
           .Ignore(x => x.Attachments)
           .Ignore(x => x.IsNewReferent)
           .AfterMap(AfterMap);

        CreateMap<Activity, ActivityDto>()
           .MapMember(x => x.Number, y => y.RowNumber)
           .MapMember(x => x.StatusLabel0, y => y.Type!.StatusLabel0)
           .MapMember(x => x.StatusLabel1, y => y.Type!.StatusLabel1)
           .MapMember(x => x.StatusLabel2, y => y.Type!.StatusLabel2);

        CreateMap<Activity, ActivityDetailDto>()
           .MapMember(x => x.Number, y => y.RowNumber)
           .MapMember(x => x.Job, y => y.Job!.Description)
           .MapMember(x => x.CustomerId, y => y.Job!.CustomerId)
           .MapMember(x => x.Customer, y => y.Job!.Customer!.Name)
           .MapMember(x => x.Address, y => y.Address != null ? y.Address.StreetAddress + ", " + y.Address.City + " (" + y.Address.Province + ")" : "")
           .MapMember(x => x.Type, y => y.Type!.Name)
           .MapMember(x => x.Referent, y => (y.Referent != null) ? y.Referent.Name : "")
           .MapMember(x => x.StatusLabel0, y => y.Type!.StatusLabel0)
           .MapMember(x => x.StatusLabel1, y => y.Type!.StatusLabel1)
           .MapMember(x => x.StatusLabel2, y => y.Type!.StatusLabel2);

        CreateMap<ActivityAttachment, ActivityAttachmentReadModel>();
        CreateMap<ActivityAttachmentReadModel, ActivityAttachment>()
           .Ignore(x => x.Activity)
           .Ignore(x => x.ActivityId)
           .IgnoreCommonMembers();

        CreateMap<ActivityAttachment, ActivityAttachmentDto>();
        CreateMap<ActivityAttachmentDto, ActivityAttachment>()
           .Ignore(x => x.Activity)
           .IgnoreCommonMembers();
    }

    private static void AfterMap(ActivityDto activityDto, Activity activity, ResolutionContext context)
    {
        if (activityDto.Attachments != null) activityDto.Attachments.Merge(activity.Attachments, (itemDto, item) => itemDto.Id == item.Id, (_, item) => item.ActivityId = activity.Id, context);
    }
}