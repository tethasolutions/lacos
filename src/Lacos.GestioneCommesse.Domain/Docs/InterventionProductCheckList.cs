﻿namespace Lacos.GestioneCommesse.Domain.Docs;

public class InterventionProductCheckList
{
    public string? Description { get; set; }
    public string? CustomerSignatureFileName { get; set; }
    public string? Notes { get; set; }

    public long InterventionProductId { get; set; }
    public InterventionProduct? InterventionProduct { get; set; }

    public ICollection<InterventionProductCheckListItem> Items { get; set; }

    public InterventionProductCheckList()
    {
        Items = new List<InterventionProductCheckListItem>();
    }
}