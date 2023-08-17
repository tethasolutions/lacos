namespace Lacos.GestioneCommesse.Framework.Configuration;

public interface ILacosConfiguration
{
    bool AllowCors { get; }
    string? CorsOrigins { get; }
    string? AttachmentsPath { get; }
}