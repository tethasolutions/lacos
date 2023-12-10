using Lacos.GestioneCommesse.Contracts.Dtos.Enums;

namespace Lacos.GestioneCommesse.Application.Docs.DTOs;

public class InterventionProductCheckListDto
{
    public long InterventionProductId { get; set; }
    public string? Description { get; set; }
    public string? CustomerSignatureFileName { get; set; }
    public string? Notes { get; set; }

    public IEnumerable<InterventionProductCheckListItemDto> Items { get; set; }
    public InterventionProductCheckListDto()
    {
        Items = new List<InterventionProductCheckListItemDto>();
    }
}