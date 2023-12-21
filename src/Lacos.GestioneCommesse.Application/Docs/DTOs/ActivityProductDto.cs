using Lacos.GestioneCommesse.Application.Shared;

namespace Lacos.GestioneCommesse.Application.Docs.DTOs;

public class ActivityProductDto : BaseEntityDto
{
    public long ProductId { get; set; }
    public long ActivityId { get; set; }
    public string? Description { get; set; }
    public string? Location { get; set; }
    public long ProductTypeId { get; set; }
}