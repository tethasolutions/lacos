using Lacos.GestioneCommesse.Application.Shared;
using Lacos.GestioneCommesse.Domain.Registry;

namespace Lacos.GestioneCommesse.Application.CheckLists.DTOs;

public class ChecklistCopyDto
{
    public long SourceChecklistId { get; set; }
    public long ActivityTypeId { get; set; }
    public long ProductTypeId { get; set; }
}