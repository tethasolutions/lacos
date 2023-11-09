namespace Lacos.GestioneCommesse.Application.Docs.DTOs;

public class JobReadModel
{
    public long Id { get; set; }
    public string? Code { get; set; }
    public DateTimeOffset Date { get; set; }
    public string? Description { get; set; }
    public string? Reference { get; set; }
    public bool HasHighPriority { get; set; }
    public JobStatusDto Status { get; set; }
    public long CustomerId { get; set; }
    public string? Customer { get; set; }
    public long? AddressId { get; set; }
    public string? Address { get; set; }
    public bool CanBeRemoved { get; set; }
}