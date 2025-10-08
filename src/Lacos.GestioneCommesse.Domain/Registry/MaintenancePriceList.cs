namespace Lacos.GestioneCommesse.Domain.Registry;

public class MaintenancePriceList : FullAuditedEntity, ILogEntity
{
    public string? Description { get; set; }
    public long HourlyRate { get; set; }
    public ICollection<MaintenancePriceListItem> Items { get; set; } 

    public MaintenancePriceList()
    {
        Items = new List<MaintenancePriceListItem>();
    }

}