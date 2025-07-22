namespace Lacos.GestioneCommesse.Domain.Registry;

public class EntityLog
{
    public long Id { get; set; }
    public string EntityType { get; set; }
    public long EntityId { get; set; }
    public string Action { get; set; }
    public DateTimeOffset Timestamp { get; set; }
    public long UserId { get; set; }
    public string? PreviousValues { get; set; }
    public string? NewValues { get; set; }

}

