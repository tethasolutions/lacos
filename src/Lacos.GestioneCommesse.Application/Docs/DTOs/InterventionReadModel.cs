﻿using Lacos.GestioneCommesse.Domain.Docs;

namespace Lacos.GestioneCommesse.Application.Docs.DTOs;

public class InterventionReadModel
{
    public long Id { get; set; }
    public InterventionStatus Status { get; set; }
    public DateTimeOffset Start { get; set; }
    public DateTimeOffset End { get; set; }
    public string? Customer { get; set; }
    public string? CustomerAddress { get; set; }
    public string? Description { get; set; }
    public IEnumerable<InterventionOperatorReadModel> Operators { get; set; }
    public string? ActivityType { get; set; }

    public InterventionReadModel()
    {
        Operators = new List<InterventionOperatorReadModel>();
    }
}

public class InterventionOperatorReadModel
{
    public long Id { get; set; }
    public string? Name { get; set; }
    public string? ColorHex { get; set; }
}