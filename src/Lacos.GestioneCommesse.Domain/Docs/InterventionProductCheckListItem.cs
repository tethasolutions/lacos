﻿using Lacos.GestioneCommesse.Domain.Registry;

namespace Lacos.GestioneCommesse.Domain.Docs;

public class InterventionProductCheckListItem : FullAuditedEntity, ILogEntity
{
    public string? Description { get; set; }
    public InterventionProductCheckListItemOutcome? Outcome { get; set; }
    public string? Notes { get; set; }
    public string? CorrectiveAction { get; set; }
    public string? AttachmentFileName { get; set; }

    public long? OperatorId { get; set; }
    public Operator? Operator { get; set; }

    public long CheckListId { get; set; }
    public InterventionProductCheckList? CheckList { get; set; }
}