using Lacos.GestioneCommesse.Contracts.Dtos.Enums;

namespace Lacos.GestioneCommesse.Contracts.Dtos.Docs
{
    public class SyncInterventionProductCheckListItemDto:SyncBaseDto
    {
        public string? Description { get; set; }
        public InterventionProductCheckListItemOutcome? Outcome { get; set; }
        public string? Notes { get; set; }
        public string? CorrectiveAction { get; set; }
        public string? AttachmentFileName { get; set; }
        public long? OperatorId { get; set; }
        public long? CheckListId { get; set; }

    }

   
}
