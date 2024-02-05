﻿using Lacos.GestioneCommesse.Application.Shared;
using Lacos.GestioneCommesse.Domain.Docs;

namespace Lacos.GestioneCommesse.Application.Docs.DTOs;

public class TicketDto : BaseEntityDto
{
    public int? Number { get; set; }
    public int Year { get; set; }
    public DateTimeOffset Date { get; set; }
    public string? Description { get; set; }
    public TicketStatus Status { get; set; }
    public long? JobId { get; set; }
    public long? ActivityId { get; set; }
    public long CustomerId { get; set; }
    public string? CustomerName { get; set; }

    public ICollection<TicketAttachmentDto> Pictures { get; set; }

    public TicketDto()
    {
        Pictures = new List<TicketAttachmentDto>();
    }
} 