using Lacos.GestioneCommesse.Domain.Security;

namespace Lacos.GestioneCommesse.Framework.Session;

public interface ILacosUser
{
    long UserId { get; }
    Role Role { get; }
    bool Enabled { get; }
    string AccessToken { get; }
    string Salt { get; }
    string PasswordHash { get; }
    string UserName { get; }
    public long? OperatorId { get; }
}