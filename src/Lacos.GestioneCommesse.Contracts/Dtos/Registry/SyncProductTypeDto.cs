namespace Lacos.GestioneCommesse.Contracts.Dtos.Registry
{
    public class SyncProductTypeDto: SyncBaseDto
    {
        public string? Code { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public bool IsReiDoor { get; set; }
        public bool IsSparePart { get; set; }

    }
}
