

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
            CreateMap<PurchaseOrder, SyncPurchaseOrderDto>();
            CreateMap<PurchaseOrderItem, SyncPurchaseOrderItemDto>();
            CreateMap<Ticket, SyncTicketDto>();
            CreateMap<TicketPicture, SyncTicketPictureDto>();
            CreateMap<ActivityType, SyncActivityTypeDto>()
                .MapMember(x=>x.OperatorIds,y=>y.Operators.Select(x=>x.Id).ToList());;
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
            CreateMap<SyncPurchaseOrderDto, PurchaseOrder>()
                .IgnoreCommonMembers()
                .IgnoreNavigationPropertyEntity();
            CreateMap<SyncPurchaseOrderItemDto, PurchaseOrderItem>()
                .IgnoreCommonMembers()
                .IgnoreNavigationPropertyEntity();
            CreateMap<SyncTicketDto, Ticket>()
                .Ignore(e => e.IsNew)
                .IgnoreCommonMembers()
                .IgnoreNavigationPropertyEntity();
            CreateMap<SyncTicketPictureDto, TicketPicture>()
                .IgnoreCommonMembers()
                .IgnoreNavigationPropertyEntity();


        }
    }
}
