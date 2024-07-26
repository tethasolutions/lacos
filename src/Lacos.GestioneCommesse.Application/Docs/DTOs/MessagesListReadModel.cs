using Lacos.GestioneCommesse.Application.Shared;
using Lacos.GestioneCommesse.Domain.Docs;
using Lacos.GestioneCommesse.Domain.Registry;

namespace Lacos.GestioneCommesse.Application.Docs.DTOs;

public class MessagesListReadModel
{
    public long id { get; set; }
    public DateTimeOffset Date { get; set; }
    public string Note { get; set; }

    public long? SenderOperatorId { get; set; }
    public string? SenderOperator { get; set; }
    public string? TargetOperators { get; set; }
    public bool? IsRead { get; set; }

    public string? JobCode { get; set; }
    public string? JobReference { get; set; }
    public string? CustomerName {  get; set; }
    public string? ActivityType { get; set; }
    public string? ActivityColor { get; set; }
    public string? ActivityDescription { get; set; }


    public long? JobId { get; set; }
    public long? ActivityId { get; set; }
    public long? TicketId { get; set; }
    public long? PurchaseOrderId { get; set; }
    public string? ElementType { get; set; }
}