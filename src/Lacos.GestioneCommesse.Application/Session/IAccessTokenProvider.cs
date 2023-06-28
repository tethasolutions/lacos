namespace Lacos.GestioneCommesse.Application.Session;

public interface IAccessTokenProvider
{
    string? AccessToken { get; }
}