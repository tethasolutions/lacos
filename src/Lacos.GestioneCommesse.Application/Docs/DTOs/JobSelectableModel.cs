using Lacos.GestioneCommesse.Domain.Docs;

namespace Lacos.GestioneCommesse.Application.Docs.DTOs;

public class JobSelectableModel
{
    public long Id { get; set; }
    public string? Code { get; set; }
    public int Year { get; set; }
    public int Number { get; set; }
    public string? Description { get; set; }
    public long CustomerId { get; set; }
    public string? Customer { get; set; }
    public long? AddressId { get; set; }
    public string? FullName { get; set; }
}