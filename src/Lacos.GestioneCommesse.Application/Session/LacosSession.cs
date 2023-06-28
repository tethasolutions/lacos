using Lacos.GestioneCommesse.Dal;
using Lacos.GestioneCommesse.Domain;
using Lacos.GestioneCommesse.Domain.Security;
using Lacos.GestioneCommesse.Framework.Session;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Lacos.GestioneCommesse.Application.Session;

public class LacosSession : ILacosSession
{
    private readonly IAccessTokenProvider accessTokenProvider;
    private readonly IServiceProvider serviceProvider;

    private string? accessToken;
    private string AccessToken => accessToken ??= accessTokenProvider.AccessToken;

    private LacosUser? currentUser;
    public ILacosUser? CurrentUser => currentUser ??= SetCurrentUser().GetAwaiter().GetResult();

    public LacosSession(
        IAccessTokenProvider accessTokenProvider,
        IServiceProvider serviceProvider
    )
    {
        this.accessTokenProvider = accessTokenProvider;
        this.serviceProvider = serviceProvider;
    }

    public bool IsAuthenticated()
    {
        return CurrentUser is { Enabled: true };
    }

    public bool IsAuthorized(Role role)
    {
        return IsAuthenticated() && CurrentUser.Role == role;
    }

    private async Task<LacosUser?> SetCurrentUser()
    {
        currentUser = await BuildUser();

        if (currentUser == null)
        {
            return currentUser;
        }

        await FillInfo();

        return currentUser;
    }

    private async Task<LacosUser?> BuildUser()
    {
        var user = await Query<User>()
            .SingleOrDefaultAsync(e => e.AccessToken == AccessToken);

        if (user == null)
        {
            return null;
        }

        var rivendellUser = new LacosUser(user.Id, user.Role, user.Enabled, user.AccessToken, user.Salt,
            user.PasswordHash, user.UserName);

        return rivendellUser;
    }

    private Task FillInfo()
    {
        // riempire qui eventuali altre proprietà di LacosUser recuperate da altre tabelle

        return Task.CompletedTask;
    }

    private IQueryable<T> Query<T>() where T : BaseEntity
    {
        return serviceProvider
            .GetRequiredService<IRepository<T>>()
            .Query()
            .AsNoTracking();
    }
}