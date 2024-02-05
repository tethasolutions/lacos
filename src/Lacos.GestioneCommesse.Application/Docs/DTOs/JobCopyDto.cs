using Lacos.GestioneCommesse.Application.Shared;
using Lacos.GestioneCommesse.Domain.Registry;

namespace Lacos.GestioneCommesse.Application.Docs.DTOs;

public class JobCopyDto : BaseEntityDto
{
    public long OriginalId { get; set; }
    public DateTimeOffset Date { get; set; }
    public string? Description { get; set; }
    public string? Reference { get; set; }
    public long CustomerId { get; set; }
    public long? AddressId { get; set; }
    public long? ReferentId { get; set; }
}