using Lacos.GestioneCommesse.Domain.Docs;
using Lacos.GestioneCommesse.Domain.Security;

namespace Lacos.GestioneCommesse.Domain.Registry;

public class NotificationOperator : FullAuditedEntity
{
    public long? OperatorId { get; set; }
    public Operator? Operator { get; set; }

}