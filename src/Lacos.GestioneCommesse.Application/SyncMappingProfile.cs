

using AutoMapper;
using Lacos.GestioneCommesse.Contracts.Dtos.Docs;
using Lacos.GestioneCommesse.Contracts.Dtos.Registry;
using Lacos.GestioneCommesse.Contracts.Dtos.Security;
using Lacos.GestioneCommesse.Domain.Docs;
using Lacos.GestioneCommesse.Domain.Registry;
using Lacos.GestioneCommesse.Domain.Security;

namespace Lacos.GestioneCommesse.Application
{
    public class SyncMappingProfile: Profile
    {
        public SyncMappingProfile()
        {
            CreateMap<Activity, SyncActivityDto>();
            CreateMap<ActivityProduct, SyncActivityProductDto>();
            CreateMap<Intervention, SyncInterventionDto>();
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
            CreateMap<ActivityType, SyncActivityTypeDto>();
            CreateMap<CheckList, SyncCheckListDto>();
            CreateMap<CheckListItem, SyncCheckListItemDto>();
            CreateMap<Customer, SyncCustomerDto>();
            CreateMap<CustomerAddress, SyncCustomerAddressDto>();
            CreateMap<Product, SyncProductDto>();
            CreateMap<ProductType, SyncProductTypeDto>();
            CreateMap<ProductDocument, SyncProductDocumentDto>();
            CreateMap<Operator, SyncOperatorDto>();
            CreateMap<OperatorDocument, SyncOperatorDocumentDto>();
            CreateMap<Vehicle, SyncVehicleDto>();
            CreateMap<User, SyncUserDto>();


        }
    }
}
