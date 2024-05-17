namespace Lacos.GestioneCommesse.Domain.Application;

public class DocumentToSyncQueue : FullAuditedEntity
{
    public string DeviceGuid { get; set; }
    public string DocumentName { get; set; }
    public bool IsSyncronized { get; set; }
    public int Order { get; set; }
    
    public DocumentToSyncQueue()
    {
       IsSyncronized = false;
       Order = int.MaxValue;
    }
}
