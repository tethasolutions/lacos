using Lacos.GestioneCommesse.Application.Shared;
using Lacos.GestioneCommesse.Domain.Registry;

namespace Lacos.GestioneCommesse.Application.Docs.DTOs;

public class CopyDto
{
    public long SourceId { get; set; }
    public long JobId { get; set; }
}