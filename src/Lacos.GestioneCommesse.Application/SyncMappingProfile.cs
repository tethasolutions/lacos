

using AutoMapper;
using Lacos.GestioneCommesse.Contracts.Dtos.Docs;
using Lacos.GestioneCommesse.Contracts.Dtos.Registry;
using Lacos.GestioneCommesse.Contracts.Dtos.Security;
using Lacos.GestioneCommesse.Domain.Docs;
using Lacos.GestioneCommesse.Domain.Registry;
using Lacos.GestioneCommesse.Domain.Security;
using Lacos.GestioneCommesse.Framework.Extensions;

namespace Lacos.GestioneCommesse.Application
{
    public class SyncMappingProfile: Profile
    {
        public SyncMappingProfile()
        {
            CreateMap<Activity, SyncActivityDto>();
            CreateMap<ActivityProduct, SyncActivityProductDto>();
            CreateMap<ActivityAttachment, SyncActivityAttachmentsDto>();

            CreateMap<Intervention, SyncInterventionDto>()
                .MapMember(x=>x.OperatorIds,y=>y.Operators.Select(x=>x.Id).ToList());
            CreateMap<InterventionDispute, SyncInterventionDisputeDto>();
            CreateMap<InterventionNote, SyncInterventionNoteDto>();
            CreateMap<InterventionProduct, SyncInterventionProductDto>();
            CreateMap<InterventionProductCheckList, SyncInterventionProductCheckListDto>();
            CreateMap<InterventionProductCheckListItem, SyncInterventionProductCheckListItemDto>();
            CreateMap<InterventionProductPicture, SyncInterventionProductPictureDto>();
            CreateMap<Job, SyncJobDto>();
            CreateMap<PurchaseOrder, SyncPurchaseOrderDto>()
                .MapMember(x => x.JobIds, y=>y.Jobs.Select(x=>x.Id).ToList());
            CreateMap<PurchaseOrderItem, SyncPurchaseOrderItemDto>();
            CreateMap<Ticket, SyncTicketDto>();
            CreateMap<TicketPicture, SyncTicketPictureDto>();
            CreateMap<ActivityType, SyncActivityTypeDto>()
                .MapMember(x=>x.OperatorIds,y=>y.Operators.Select(x=>x.Id).ToList())
                .MapMember(x=>x.ActivitiesIds,y=>y.Activities.Select(x=>x.Id).ToList())
                .MapMember(x=>x.CheckListsIds,y=>y.CheckLists.Select(x=>x.Id).ToList());
            
            CreateMap<CheckList, SyncCheckListDto>();
            CreateMap<CheckListItem, SyncCheckListItemDto>();
            CreateMap<Customer, SyncCustomerDto>();
            CreateMap<Supplier, SyncSupplierDto>();
            CreateMap<Address, SyncAddressDto>();
            CreateMap<Product, SyncProductDto>();
            CreateMap<ProductType, SyncProductTypeDto>();
            CreateMap<ProductDocument, SyncProductDocumentDto>();
            CreateMap<Operator, SyncOperatorDto>();
            CreateMap<OperatorDocument, SyncOperatorDocumentDto>();
            CreateMap<Vehicle, SyncVehicleDto>();
            CreateMap<User, SyncUserDto>();
            CreateMap<MaintenancePriceList, SyncMaintenancePriceListDto>();
         
            
            CreateMap<SyncInterventionDto, Intervention>()
                .IgnoreCommonMembers()
                .IgnoreNavigationPropertyEntity();
            CreateMap<SyncInterventionDisputeDto, InterventionDispute>()
                .IgnoreCommonMembers().IgnoreNavigationPropertyEntity();
            CreateMap<SyncInterventionNoteDto, InterventionNote>()
                .IgnoreCommonMembers()
                .IgnoreNavigationPropertyEntity();
            CreateMap<SyncInterventionProductDto, InterventionProduct>()
                .IgnoreCommonMembers()
                .IgnoreNavigationPropertyEntity();
            CreateMap<SyncInterventionProductCheckListDto, InterventionProductCheckList>()
                .IgnoreCommonMembers()
                .IgnoreNavigationPropertyEntity();
            CreateMap<SyncInterventionProductCheckListItemDto, InterventionProductCheckListItem>()
                .IgnoreCommonMembers()
                .IgnoreNavigationPropertyEntity();
            CreateMap<SyncInterventionProductPictureDto, InterventionProductPicture>()
                .IgnoreCommonMembers()
                .IgnoreNavigationPropertyEntity();
            CreateMap<SyncProductDto, Product>()
                .Ignore(x => x.Note)
                .Ignore(x => x.Brand)
                .Ignore(x => x.Side)
                .Ignore(x => x.Size)
                .Ignore(x => x.Material)
                .Ignore(x => x.DefaultPrice)
                .Ignore(x => x.MonthlyMaintenance)
                .Ignore(x => x.IsDecommissioned)
                .IgnoreCommonMembers()
                .IgnoreNavigationPropertyEntity();
            CreateMap<SyncPurchaseOrderDto, PurchaseOrder>()
                .Ignore(e => e.ExpectedDate)
                .Ignore(e => e.ActivityTypeId)
                .IgnoreCommonMembers()
                .IgnoreNavigationPropertyEntity();
            CreateMap<SyncPurchaseOrderItemDto, PurchaseOrderItem>()
                .Ignore(x => x.UnitPrice)
                .Ignore(x => x.TotalAmount)
                .IgnoreCommonMembers()
                .IgnoreNavigationPropertyEntity();
            CreateMap<SyncTicketDto, Ticket>()
                .Ignore(e => e.IsNew)
                .Ignore(e => e.ActivityId)
                .Ignore(e => e.PurchaseOrderId)
                .IgnoreCommonMembers()
                .IgnoreNavigationPropertyEntity();
            CreateMap<SyncTicketPictureDto, TicketPicture>()
                .IgnoreCommonMembers()
                .IgnoreNavigationPropertyEntity();


        }
    }
}
