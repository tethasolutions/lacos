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

        public bool ChangesHaveRecord =>
            (InterventionProductPictures.Any()) ||
            (InterventionDisputes.Any() ) ||
            (InterventionNotes.Any() ) ||
            (InterventionProducts.Any()) ||
            (InterventionProductCheckLists.Any()) ||
            (InterventionProductCheckListItems.Any()) ||
            (InterventionProductPictures.Any()) ||
            (PurchaseOrders.Any()) ||
            (PurchaseOrderItems.Any()) ||
            (Tickets.Any()) ||
            (TicketPictures.Any());

        public bool ChangesHaveNewRecord => ChangesHaveRecord && 
                                            (InterventionProductPictures.Any(x => x.Id < 0)) ||
                                            (InterventionDisputes.Any(x => x.Id < 0)) ||
                                            (InterventionNotes.Any(x => x.Id < 0)) ||
                                            (InterventionProducts.Any(x => x.Id < 0)) ||
                                            (InterventionProductCheckLists.Any(x => x.Id < 0)) ||
                                            (InterventionProductCheckListItems.Any(x => x.Id < 0)) ||
                                            (InterventionProductPictures.Any(x => x.Id < 0)) ||
                                            (PurchaseOrders.Any(x => x.Id < 0)) ||
                                            (PurchaseOrderItems.Any(x => x.Id < 0)) ||
                                            (Tickets.Any(x => x.Id < 0)) ||
                                            (TicketPictures.Any(x => x.Id < 0));
    }
}
