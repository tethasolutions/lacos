using Lacos.GestioneCommesse.Framework.Configuration;

namespace Lacos.GestioneCommesse.WebApi.Configuration;

public class LacosConfiguration : ILacosConfiguration
{
    public bool AllowCors { get; set; }
    public string? CorsOrigins { get; set; }
    public string? AttachmentsPath { get; set; }
    public int? RescheduleActivityId { get; set; }
}