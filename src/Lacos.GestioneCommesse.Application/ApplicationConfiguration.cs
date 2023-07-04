using Lacos.GestioneCommesse.Application.Customers.Services;
using Lacos.GestioneCommesse.Application.Security;
using Lacos.GestioneCommesse.Application.Session;
using Microsoft.Extensions.DependencyInjection;

namespace Lacos.GestioneCommesse.Application;

public static class ApplicationConfiguration
{
    public static IServiceCollection AddApplication<TAccessTokenProvider>(this IServiceCollection services)
        where TAccessTokenProvider : class, IAccessTokenProvider
    {
        services
            .AddScoped<ISecurityContextFactory, SecurityContextFactory>()
            .AddScoped<ISecurityService, SecurityService>()
            .AddScoped<IAccessTokenProvider, TAccessTokenProvider>()
            .AddScoped<IContactService, ContactService>()
            .AddScoped<IAddressService, AddressService>();

        return services;
    }
}