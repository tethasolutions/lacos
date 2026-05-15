namespace Lacos.GestioneCommesse.Domain;

public class AppLog : BaseEntity
{
    public DateTimeOffset Timestamp { get; set; }
    public string? Endpoint { get; set; }
    public string? Data { get; set; }
}
