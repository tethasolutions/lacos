namespace Lacos.GestioneCommesse.Application.Docs.DTOs;

public class InterventionProductReadModel
{
    public long Id { get; set; }
    public string? Type { get; set; }
    public string? Code { get; set; }
    public string? Name { get; set; }
    public string? PictureFileName { get; set; }
    public string? QrCode { get; set; }
    public long ActivityId { get; set; }
    public long? InterventionId { get; set; }
    public DateTimeOffset? InterventionStart { get; set; }
    public DateTimeOffset? InterventionEnd { get; set; }
    public IEnumerable<string> InterventionOperators { get; set; }
    public bool CanBeRemoved { get; set; }

    public InterventionProductReadModel()
    {
        InterventionOperators = new List<string>();
    }
}