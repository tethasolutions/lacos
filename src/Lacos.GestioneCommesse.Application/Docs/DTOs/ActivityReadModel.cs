namespace Lacos.GestioneCommesse.Application.Docs.DTOs;

public class ActivityReadModel
{
    public long Id { get; set; }
    public int Number { get; set; }
    public long JobId { get; set; }
    public string? Description { get; set; }
    public ActivityStatusDto Status { get; set; }
    public string? CustomerAddress { get; set; }
    public string? Type { get; set; }
    public string? Source { get; set; }
    public bool CanBeRemoved { get; set; }
}