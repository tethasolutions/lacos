using Lacos.GestioneCommesse.Application.Shared;
using Lacos.GestioneCommesse.Domain;
using Lacos.GestioneCommesse.Domain.Docs;
using Lacos.GestioneCommesse.Domain.Registry;

namespace Lacos.GestioneCommesse.Application.Docs.DTOs;

public class MessageNotificationReadModel
{
    public long MessageId { get; set; }

    public long OperatorId { get; set; }

    public bool IsRead { get; set; }

}

public class MessageNotificationCounterDto : BaseEntityDto
{
    public int MessagesUnread { get; set; }
}