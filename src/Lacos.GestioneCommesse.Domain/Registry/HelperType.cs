namespace Lacos.GestioneCommesse.Domain.Registry;

public class HelperType : FullAuditedEntity
{
    public string Type { get; set; }
    public ICollection<HelperDocument> HelperDocuments { get; set; }

    public HelperType()
    {
        HelperDocuments = new List<HelperDocument>();
    }
}