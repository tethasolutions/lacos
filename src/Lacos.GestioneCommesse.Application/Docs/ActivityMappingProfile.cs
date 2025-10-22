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
           .MapMember(x => x.JobMandatoryDate, y => y.Job!.MandatoryDate)
           .MapMember(x => x.JobIsInLate, y => (y.Job!.Status != JobStatus.InProgress && y.Job!.Status != JobStatus.Pending)
                ? false
                : (y.Job!.MandatoryDate == null
                    ? false
                    : y.Job!.MandatoryDate.Value.AddHours(2).AddDays(-5) < DateTimeOffset.Now.Date))
           .MapMember(x => x.CustomerId, y => y.Job!.CustomerId)
           .MapMember(x => x.Customer, y => y.Job!.Customer == null ? null : y.Job.Customer.Name)
           .MapMember(x => x.ActivityColor, y => y.Type!.ColorHex)
           .MapMember(x => x.LastOperator, y => y.CreatedBy)
           .MapMember(x => x.ReferentName, y => (y.Referent != null) ? y.Referent.Name : "")
           .MapMember(x => x.HasAttachments, y => y.Attachments.Any())
           .MapMember(x => x.IsExpired, y => (y.Status == ActivityStatus.Completed || y.Status == ActivityStatus.Ready)
                ? false
                : (y.ExpirationDate == null
                    ? false
                    : y.ExpirationDate.Value.AddHours(2).AddDays(-5) < DateTimeOffset.Now.Date))
           .MapMember(x => x.IsInternal, y => y.Type!.IsInternal)
           .MapMember(x => x.IsFromTicket, y => y.Job!.Tickets.Any(t => t.ActivityId == y.Id))
           .MapMember(x => x.StatusLabel0, y => y.Type!.StatusLabel0)
           .MapMember(x => x.StatusLabel1, y => y.Type!.StatusLabel1)
           .MapMember(x => x.StatusLabel2, y => y.Type!.StatusLabel2)
           .MapMember(x => x.StatusLabel3, y => y.Type!.StatusLabel3)
           .MapMember(x => x.UnreadMessages, y => y.Messages.SelectMany(e => e.MessageNotifications).Count(e => !e.IsRead))
           .MapMember(x => x.CreatedOn, y => y.CreatedOn)
           .MapMember(x => x.EditedOn, y => y.EditedOn)
           .MapMember(x => x.CanHaveDependencies, y => y.Type!.HasDependencies)
           .MapMember(x => x.HasDependencies, y => y.ActivityDependencies.Any() || y.PurchaseOrderDependencies.Any())
           .MapMember(x => x.TotalDependencies, y => (y.ActivityDependencies.Any() ? y.ActivityDependencies.Count() : 0) + 
                (y.PurchaseOrderDependencies.Any() ? y.PurchaseOrderDependencies.Count() : 0))
           .MapMember(x => x.FulfilledDependencies, y => (y.ActivityDependencies.Any(a => a.Status >= ActivityStatus.Ready) ? y.ActivityDependencies.Count(a => a.Status >= ActivityStatus.Ready) : 0) +
                (y.PurchaseOrderDependencies.Any(p => p.Status == PurchaseOrderStatus.Completed) ? y.PurchaseOrderDependencies.Count(p => p.Status == PurchaseOrderStatus.Completed) : 0))
           .MapMember(x => x.PurchaseOrderStatus, y => (PurchaseOrderStatus?)(
                y.PurchaseOrderDependencies.Any() ? 
                    y.PurchaseOrderDependencies.Any(p => p.Status == PurchaseOrderStatus.Completed || p.Status == PurchaseOrderStatus.Partial)
                    && !y.PurchaseOrderDependencies.All(p => p.Status == PurchaseOrderStatus.Completed) ?
                        PurchaseOrderStatus.Partial
                        : y.PurchaseOrderDependencies.Max(p => p.Status)
                :
                !y.Job.PurchaseOrders.Where(p => !p.ParentActivities.Any() && p.ActivityTypeId == y.TypeId).Any() ? 
                    null 
                    : 
                    (y.Job.PurchaseOrders.Where(p => !p.ParentActivities.Any() && p.ActivityTypeId == y.TypeId).Any(p => p.Status == PurchaseOrderStatus.Completed || p.Status == PurchaseOrderStatus.Partial) 
                    && !y.Job.PurchaseOrders.Where(p => !p.ParentActivities.Any() && p.ActivityTypeId == y.TypeId).All(p => p.Status == PurchaseOrderStatus.Completed) ?
                        PurchaseOrderStatus.Partial
                        : y.Job.PurchaseOrders.Where(p => !p.ParentActivities.Any() && p.ActivityTypeId == y.TypeId).Max(p => p.Status)
                    )
                )
            );

        CreateMap<ActivityDto, Activity>()
           .IgnoreCommonMembers()
           .Ignore(x => x.RowNumber)
           .Ignore(x => x.Supplier)
           .Ignore(x => x.Address)
           //.MapMember(x => x.JobId, (x, y) => y.IsTransient() ? x.JobId : y.JobId)
           .Ignore(x => x.Job)
           .MapMember(x => x.TypeId, (x, y) => y.IsTransient() ? x.TypeId : y.TypeId)
           .Ignore(x => x.Type)
           .Ignore(x => x.Interventions)
           .Ignore(x => x.ActivityProducts)
           .Ignore(x => x.Referent)
           .Ignore(x => x.Attachments)
           .Ignore(x => x.IsNewReferent)
           .Ignore(x => x.Messages)
           .Ignore(x => x.ParentActivities)
           .Ignore(x => x.ActivityDependencies)
           .Ignore(x => x.PurchaseOrderDependencies)
           .AfterMap(AfterMap);

        CreateMap<Activity, ActivityDto>()
           .MapMember(x => x.Number, y => y.RowNumber)
           .MapMember(x => x.StatusLabel0, y => y.Type!.StatusLabel0)
           .MapMember(x => x.StatusLabel1, y => y.Type!.StatusLabel1)
           .MapMember(x => x.StatusLabel2, y => y.Type!.StatusLabel2)
           .MapMember(x => x.StatusLabel3, y => y.Type!.StatusLabel3)
           .MapMember(x => x.CanHaveDependencies, y => y.Type!.HasDependencies);

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
           .MapMember(x => x.StatusLabel2, y => y.Type!.StatusLabel2)
           .MapMember(x => x.StatusLabel3, y => y.Type!.StatusLabel3)
           .MapMember(x => x.CanHaveDependencies, y => y.Type!.HasDependencies)
           .MapMember(x => x.HasDependencies, y => y.ActivityDependencies.Any() || y.PurchaseOrderDependencies.Any())
           .MapMember(x => x.TotalDependencies, y => (y.ActivityDependencies.Any() ? y.ActivityDependencies.Count() : 0) +
                (y.PurchaseOrderDependencies.Any() ? y.PurchaseOrderDependencies.Count() : 0))
           .MapMember(x => x.FulfilledDependencies, y => (y.ActivityDependencies.Any(a => a.Status >= ActivityStatus.Ready) ? y.ActivityDependencies.Count(a => a.Status >= ActivityStatus.Ready) : 0) +
                (y.PurchaseOrderDependencies.Any(p => p.Status == PurchaseOrderStatus.Completed) ? y.PurchaseOrderDependencies.Count(p => p.Status == PurchaseOrderStatus.Completed) : 0))
           .MapMember(x => x.HasUnpaidAccounts, y => y.Job!.Accountings.Any(a => a.AccountingType.GenerateAlert && !a.IsPaid));

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
        if (activityDto.Messages != null) activityDto.Messages.Merge(activity.Messages, (itemDto, item) => itemDto.Id == item.Id, (_, item) => item.ActivityId = activity.Id, context);
    }
}