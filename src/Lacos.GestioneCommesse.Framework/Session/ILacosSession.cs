using Lacos.GestioneCommesse.Domain.Security;

namespace Lacos.GestioneCommesse.Framework.Session;

public interface ILacosSession
{
    ILacosUser? CurrentUser { get; }

    bool IsAuthenticated();
    bool IsAuthorized(Role role);
}