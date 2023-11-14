﻿using Lacos.GestioneCommesse.Application.Shared;
using Lacos.GestioneCommesse.Domain.Docs;
using Lacos.GestioneCommesse.Domain.Registry;

namespace Lacos.GestioneCommesse.Application.Docs.DTOs;

public class PurchaseOrderDto : BaseEntityDto
{
    public int? Number { get; set; }
    public int Year { get; set; }
    public DateTimeOffset Date { get; set; }
    public string? Description { get; set; }
    public PurchaseOrderStatus Status { get; set; }

    public long? JobId { get; set; }

    public long SupplierId { get; set; }
    public string? SupplierName { get; set; }

    public ICollection<PurchaseOrderItem> Items { get; set; }

    public PurchaseOrderDto()
    {
        Items = new List<PurchaseOrderItem>();
    }
} 