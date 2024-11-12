namespace Lacos.GestioneCommesse.Domain.Registry;

public class HelperDocument : FullAuditedEntity
{
    public long? HelperTypeId { get; set; }
    public HelperType HelperType { get; set; }
    public string Description { get; set; }
    public string? FileName { get; set; }


}