using Lacos.GestioneCommesse.Domain.Docs;
using Lacos.GestioneCommesse.Domain.Security;

namespace Lacos.GestioneCommesse.Domain.Registry;

public class GlobalSetting : FullAuditedEntity, ILogEntity
{
    public GlobalSettingType? Type { get; set; }
    public long? ValueNumber { get; set; }
    public string? ValueString { get; set; }
}

public enum GlobalSettingType
{
    FloorDeliverySupplierId,
    FloorDeliveryExpenseAmount,
    FloorDeliveryExpenseNote
}