using Lacos.GestioneCommesse.Application.Shared;
using Lacos.GestioneCommesse.Domain.Docs;

namespace Lacos.GestioneCommesse.Application.Docs.DTOs;

public class JobDto : BaseEntityDto
{
    public int? Number { get; set; }
    public int? Year { get; set; }
    public DateTimeOffset Date { get; set; }
    public string? Description { get; set; }
    public JobStatus Status { get; set; }

    public long CustomerId { get; set; }
}