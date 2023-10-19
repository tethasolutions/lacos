using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lacos.GestioneCommesse.Contracts.Dtos.Docs;

namespace Lacos.GestioneCommesse.Contracts.Dtos.Application
{
    public class SyncLocalDbChanges
    {
        public List<SyncInterventionDto> Interventions { get; set; }
        public List<SyncInterventionDisputeDto> InterventionDisputes { get; set; }
        public List<SyncInterventionNoteDto> InterventionNotes { get; set; }
        public List<SyncInterventionProductDto> InterventionProducts { get; set; }
        public List<SyncInterventionProductCheckListDto> InterventionProductCheckLists { get; set; }
        public List<SyncInterventionProductCheckListItemDto> InterventionProductCheckListItems { get; set; }
        public List<SyncInterventionProductPictureDto> InterventionProductPictures { get; set; }   
        public List<SyncPurchaseOrderDto> PurchaseOrders { get; set; }
        public List<SyncPurchaseOrderItemDto> PurchaseOrderItems { get; set;}
        public List<SyncTicketDto> Tickets { get; set; }
        public List<SyncTicketPictureDto> TicketPictures { get; set; }
    }
}
