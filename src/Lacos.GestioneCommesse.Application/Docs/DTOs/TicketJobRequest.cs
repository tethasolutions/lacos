using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lacos.GestioneCommesse.Application.Docs.DTOs;

    public class TicketJobRequest
{
    public long CustomerId { get; set; }
    public long? AddressId { get; set; }
    public string? TicketCode { get; set; }
    public string? TicketDescription { get; set; }
}

