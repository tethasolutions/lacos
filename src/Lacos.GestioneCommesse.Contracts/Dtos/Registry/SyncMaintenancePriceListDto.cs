namespace Lacos.GestioneCommesse.Contracts.Dtos.Registry;

public class SyncMaintenancePriceListDto : SyncBaseDto
{
    public string? Description { get; set; }
    public long HourlyRate { get; set; }

    public SyncMaintenancePriceListDto()
    {
    
    }

}