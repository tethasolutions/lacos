using Lacos.GestioneCommesse.Domain.Registry;

namespace Lacos.GestioneCommesse.Domain.Docs;

public class MessageNotification : FullAuditedEntity, IOperatorEntity
{
    public long MessageId { get; set; }
    public Message? Message { get; set; }

    public long OperatorId { get; set; }
    public Operator? Operator { get; set; }

    public DateTimeOffset? ReadDate { get; set; }
    public bool IsRead { get; set; }

}