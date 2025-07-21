using Lacos.GestioneCommesse.Application.Shared;
using Lacos.GestioneCommesse.Domain.Docs;
using Lacos.GestioneCommesse.Domain.Registry;

namespace Lacos.GestioneCommesse.Application.Docs.DTOs;

public class TicketReadModel : BaseEntityDto
{
    public long Id { get; set; }
    public string? Code { get; set; }
    public DateTimeOffset Date { get; set; }
    public string? Description { get; set; }
    public TicketStatus Status { get; set; }
    public long? JobId { get; set; }
    public long? AddressId { get; set; }
    public long? ActivityId { get; set; }
    public long? PurchaseOrderId { get; set; }
    public long CustomerId { get; set; }
    public string? CustomerName { get; set; }
    public string? CustomerEmail { get; set; }
    public string? OperatorName { get; set; }
    //public bool CanBeRemoved { get; set; }
    public int? UnreadMessages { get; set; }
    public bool HasInterventions { get; set; }

}