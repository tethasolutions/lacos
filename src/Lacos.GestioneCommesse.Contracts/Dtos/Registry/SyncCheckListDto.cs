namespace Lacos.GestioneCommesse.Contracts.Dtos.Registry
{
    public class SyncCheckListDto:SyncBaseDto
    {
        public string? PictureFileName { get; set; }
        public string? Description { get; set; }
        public long? ProductTypeId { get; set; }
        public long? ActivityTypeId { get; set; }
    }
}
