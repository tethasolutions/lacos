namespace Lacos.GestioneCommesse.Contracts.Dtos.Registry;

public class SyncOperatorDto:SyncBaseDto
{
    public string? Email { get; set; }
    public string? ColorHex { get; set; }
    public string? Name { get; set; }
    public long? DefaultVehicleId { get; set; }
    public long? UserId { get; set; }

}