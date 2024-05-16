using Lacos.GestioneCommesse.Domain.Registry;

namespace Lacos.GestioneCommesse.Domain.Docs;

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
