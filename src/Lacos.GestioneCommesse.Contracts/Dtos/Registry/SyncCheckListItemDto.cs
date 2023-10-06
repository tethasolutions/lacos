namespace Lacos.GestioneCommesse.Contracts.Dtos.Registry
{
    public class SyncCheckListItemDto:SyncBaseDto
    {
        public string? Description { get; set; }
        public long? CheckListId { get; set; }
    }
}
