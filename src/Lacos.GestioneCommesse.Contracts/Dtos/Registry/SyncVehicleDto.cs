namespace Lacos.GestioneCommesse.Contracts.Dtos.Registry
{
    public class SyncVehicleDto:SyncBaseDto
    {
        public string? Name { get; set; }
        public string? Plate { get; set; }
        public string? Notes { get; set; }
    }
}
