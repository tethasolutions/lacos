namespace Lacos.GestioneCommesse.Application.Docs.DTOs;

public class ActivityProductDto
{
    public long ProductId { get; set; }
    public long ActivityId { get; set; }
    public string? Description { get; set; }
    public string? Location { get; set; }
}