namespace Lacos.GestioneCommesse.Domain.Registry;

public class MaintenancePriceListItem : FullAuditedEntity, ILogEntity
{
    public long MaintenancePriceListId { get; set; }
    public MaintenancePriceList? MaintenancePriceList { get; set; }

    public string? Description { get; set; }
    public decimal ServiceCallFee { get; set; }
    public decimal TravelFee { get; set; }
    public int LimitKm { get; set; }
    public decimal ExtraFee { get; set; }

}