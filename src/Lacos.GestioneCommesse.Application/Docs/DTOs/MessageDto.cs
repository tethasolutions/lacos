﻿using Lacos.GestioneCommesse.Application.Shared;
using Lacos.GestioneCommesse.Domain.Docs;
using Lacos.GestioneCommesse.Domain.Registry;

namespace Lacos.GestioneCommesse.Application.Docs.DTOs;

public class MessageDto : BaseEntityDto
{
    public DateTimeOffset Date { get; set; }

    public string Note { get; set; }

    public long OperatorId { get; set; }

    public long? JobId { get; set; }
    public long? ActivityId { get; set; }
    public long? TicketId { get; set; }
    public long? PurchaseOrderId { get; set; }
    public bool IsFromApp { get; set; }
}