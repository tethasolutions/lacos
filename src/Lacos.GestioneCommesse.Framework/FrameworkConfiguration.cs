using Lacos.GestioneCommesse.Framework.Common;
using Lacos.GestioneCommesse.Framework.Configuration;
using Lacos.GestioneCommesse.Framework.IO;
using Lacos.GestioneCommesse.Framework.Security;
using Lacos.GestioneCommesse.Framework.Session;
using Microsoft.Extensions.DependencyInjection;

namespace Lacos.GestioneCommesse.Framework;

public static class FrameworkConfiguration
{
    public static IServiceCollection AddFramework<TSession>(this IServiceCollection services, ILacosConfiguration configuration)
        where TSession : class, ILacosSession
    {
        services
            .AddSingleton<IPasswordHasher, PasswordHasher>()
            .AddSingleton<IAccessTokenGenerator, AccessTokenGenerator>()
            .AddSingleton<IPasswordGenerator, PasswordGenerator>()
            .AddScoped<ILacosSession, TSession>()
            .AddSingleton<IGuidGenerator, GuidGenerator>()
            .AddSingleton(configuration)
            .AddSingleton<IMimeTypeProvider, MimeTypeProvider>();;

        return services;
    }
}