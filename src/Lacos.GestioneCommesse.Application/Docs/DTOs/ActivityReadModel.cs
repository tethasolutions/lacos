﻿using Lacos.GestioneCommesse.Domain.Docs;

namespace Lacos.GestioneCommesse.Application.Docs.DTOs;

public class ActivityReadModel
{
    public long Id { get; set; }
    public int Number { get; set; }
    public long JobId { get; set; }
    public string? JobCode { get; set; }
    public string? JobReference { get; set; }
    public bool? JobHasHighPriority { get; set; }
    public string? ShortDescription { get; set; }
    public ActivityStatus Status { get; set; }
    public string? Address { get; set; }
    public long? CustomerId { get; set; }
    public string? Customer { get; set; }
    public long TypeId { get; set; }
    public string? Type { get; set; }
    public string? ActivityColor { get; set; }
    public bool CanBeRemoved { get; set; }
    public DateTimeOffset? StartDate { get; set; }
    public DateTimeOffset? ExpirationDate { get; set; }
    public string? LastOperator { get; set; }
    public long? ReferentId { get; set; }
    public string? ReferentName { get;set; }
    public bool HasAttachments { get; set; }
    public bool IsNewReferent { get; set; }
    public bool IsExpired { get; set; }
    public bool IsInternal { get; set; }

    public int? UnreadMessages { get; set; }

    public string? StatusLabel0 { get; set; }
    public string? StatusLabel1 { get; set; }
    public string? StatusLabel2 { get; set; }
    public string? StatusLabel3 { get; set; }
}