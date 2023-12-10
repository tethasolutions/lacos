using Lacos.GestioneCommesse.Contracts.Dtos.Enums;

namespace Lacos.GestioneCommesse.Application.Docs.DTOs;

public class InterventionProductCheckListItemDto
{
    public string? Description { get; set; }
    public string? Outcome { get; set; }
    public string? Notes { get; set; }
    public string? OperatorName { get; set; }
}