using Lacos.GestioneCommesse.Domain.Security;
using Lacos.GestioneCommesse.Framework.Session;

namespace Lacos.GestioneCommesse.Application.Session;

public class LacosUser : ILacosUser
{
    public long UserId { get; }
    public Role Role { get; }
    public bool Enabled { get; }
    public string AccessToken { get; }
    public string Salt { get; }
    public string PasswordHash { get; }
    public string UserName { get; }

    public LacosUser(long userId, Role role, bool enabled, string accessToken,
        string salt, string passwordHash, string userName)
    {
        UserId = userId;
        Role = role;
        Enabled = enabled;
        AccessToken = accessToken;
        Salt = salt;
        PasswordHash = passwordHash;
        UserName = userName;
    }
}