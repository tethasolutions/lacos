using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Westcar.WebApplication.Dal;

namespace Lacos.GestioneCommesse.Dal;

public static class DalConfiguration
{
    public static IServiceCollection AddDal(this IServiceCollection services)
    {
        services
            .AddSingleton(DbContextOptionsFactory)
            .AddScoped<ILacosDbContext, LacosDbContext>()
            .AddScoped(typeof(IRepository<>), typeof(Repository<>))
            .AddScoped(typeof(IViewRepository<>), typeof(ViewRepository<>));

        return services;
    }

    private static DbContextOptions DbContextOptionsFactory(IServiceProvider serviceProvider)
    {
        var configuration = serviceProvider.GetRequiredService<IConfiguration>();
        var connectionString = configuration.GetConnectionString("Default");

        return LacosDbContextFactory.CreateDbContextOptions(connectionString);
    }
}