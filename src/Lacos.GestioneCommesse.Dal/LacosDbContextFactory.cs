using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Lacos.GestioneCommesse.Dal;

public class LacosDbContextFactory : IDesignTimeDbContextFactory<LacosDbContext>
{
    public LacosDbContext CreateDbContext(string[] args)
    {
        var options = CreateDbContextOptions(GetConnectionString());

        return new LacosDbContext(options, null);
    }

    public static DbContextOptions<LacosDbContext> CreateDbContextOptions(string connectionString)
    {
        var builder = new DbContextOptionsBuilder<LacosDbContext>();

        builder.UseSqlServer(connectionString, e => {
            e.CommandTimeout(3600);
            e.UseCompatibilityLevel(120);
        });

        return builder.Options;
    }

    private static string GetConnectionString()
    {
        var basePath = AppContext.BaseDirectory;
        var builder = new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile("appsettings.json");
        var config = builder.Build();

        return config.GetConnectionString("Default");
    }
}