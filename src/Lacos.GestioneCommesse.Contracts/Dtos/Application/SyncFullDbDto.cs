using Lacos.GestioneCommesse.Contracts.Dtos.Docs;
using Lacos.GestioneCommesse.Contracts.Dtos.Registry;

namespace Lacos.GestioneCommesse.Contracts.Dtos.Application
{
    public class SyncFullDbDto
    {

        public List<SyncActivityDto> Activities { get; set; }
        public List<SyncActivityProductDto> ActivityProducts { get; set; }
        public List<SyncInterventionDto> Interventions { get; set; }
        public List<SyncInterventionDisputeDto> InterventionDisputes { get; set; }
        public List<SyncInterventionNoteDto> InterventionNotes { get; set; }
        public List<SyncInterventionProductDto> InterventionProducts { get; set; }
        public List<SyncInterventionProductCheckListDto> InterventionProductCheckLists { get; set; }
        public List<SyncInterventionProductCheckListItemDto> InterventionProductCheckListItems { get; set; }
        public List<SyncInterventionProductPictureDto> InterventionProductPictures { get; set; }   
        public List<SyncJobDto> Jobs { get; set; }
        public List<SyncPurchaseOrderDto> PurchaseOrders { get; set; }
        public List<SyncPurchaseOrderItemDto> PurchaseOrderItems { get; set;}
        public List<SyncTicketDto> Tickets { get; set; }
        public List<SyncTicketPictureDto> TicketPictures { get; set; }
        public List<SyncActivityTypeDto> ActivityTypes { get; set; }
        public List<SyncCheckListDto> CheckLists { get; set; }
        public List<SyncCheckListItemDto>  CheckListItems { get; set; }
        public List<SyncCustomerDto> Customers { get; set; }
        public List<SyncCustomerAddressDto> CustomerAddresses { get; set; }
        public List<SyncOperatorDocumentDto> OperatorDocuments { get; set; }
        public List<SyncProductDto> Products { get; set; }
        public List<SyncProductDocumentDto> ProductDocuments { get; set; }
        public List<SyncProductTypeDto> ProductTypes { get; set; }
        public List<SyncVehicleDto> Vehicles { get; set; }

    }
}
