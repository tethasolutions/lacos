using AutoMapper;
using Lacos.GestioneCommesse.Framework.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using System.Net;
using Lacos.GestioneCommesse.Application;
using Lacos.GestioneCommesse.Application.Session;
using Lacos.GestioneCommesse.Dal;
using Lacos.GestioneCommesse.Framework;
using Lacos.GestioneCommesse.Framework.Configuration;
using Lacos.GestioneCommesse.WebApi.Auth;
using Lacos.GestioneCommesse.WebApi.Configuration;
using Lacos.GestioneCommesse.WebApi.ModelBinders;
using Newtonsoft.Json;

namespace Lacos.GestioneCommesse.WebApi;

public static class WebApiConfiguration
{
    public static IServiceCollection AddWebApi(this IServiceCollection services, IConfiguration configuration)
    {
        // MVC
        services
            .AddControllers(e =>
            {
                e.Filters.Add<AuthorizeFilter>();
                e.ModelBinderProviders.Insert(0, new DateTimeOffsetModelBinderProvider());
            })
            .ConfigureApiBehaviorOptions(e => e.SuppressModelStateInvalidFilter = true)
            .AddControllersAsServices()
            .AddNewtonsoftJson(e => SetupJsonSettings(e.SerializerSettings));

        // Get configuration from appsettings.json
        var lacosConfiguration = PerformBaseSetup(services, configuration);

        // App
        services
            .AddHttpContextAccessor()
            .AddFramework<LacosSession>(lacosConfiguration)
            .AddDal()
            .AddApplication<AccessTokenProvider>()
            .AddMappings();

        return services;
    }

    private static void SetupJsonSettings(JsonSerializerSettings settings)
    {
        settings.DateParseHandling = DateParseHandling.DateTimeOffset;
        settings.DateTimeZoneHandling = DateTimeZoneHandling.RoundtripKind;
    }

    private static void AddMappings(this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(ApplicationConfiguration).Assembly);

        using (var serviceProvider = services.BuildServiceProvider())
        {
            serviceProvider
                .GetRequiredService<IMapper>().ConfigurationProvider
                .AssertConfigurationIsValid();
        }
    }

    private static ILacosConfiguration PerformBaseSetup(IServiceCollection services,
        IConfiguration configuration)
    {
        var section = configuration.GetSection("Lacos");
        var lacosConfiguration = section.Get<LacosConfiguration>();

        if (lacosConfiguration == null)
        {
            throw new LacosException("Configuration section 'Lacos' not found.");
        }

        if (lacosConfiguration is { AllowCors: true })
        {
            AddCors(services, lacosConfiguration);
        }

        return lacosConfiguration;
    }

    private static void AddCors(IServiceCollection services, ILacosConfiguration configuration)
    {
        var origins = configuration.CorsOrigins?.Split(',', StringSplitOptions.RemoveEmptyEntries)
                      ?? Array.Empty<string>();

        services.AddCors(e =>
            e.AddPolicy("Lacos",
                p => p.AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials()
                    .WithOrigins(origins)
            )
        );
    }


    public static IApplicationBuilder UseWebApi(this IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app
                .UseDeveloperExceptionPage();
        }
        else
        {
            app
                .UseHsts()
                .UseHttpsRedirection();
        }

        app
            .UseDefaultFiles()
            .UseStaticFiles()
            .UseRouting()
            .UseCors("Lacos")
            .UseExceptionHandler(appError => appError.Run(HandleError))
            .UseAuthorization()
            .UseAuthentication()
            .UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

        return app;
    }

    private static async Task HandleError(HttpContext context)
    {
        var response = context.Response;
        var feature = context.Features.Get<IExceptionHandlerFeature>();
        var error = feature?.Error is AggregateException
            ? feature.Error.InnerException
            : feature?.Error;
        var configuration = context.RequestServices.GetRequiredService<ILacosConfiguration>();

        if (configuration.AllowCors)
        {
            response.Headers.Add("access-control-allow-credentials", "true");
            response.Headers.Add("access-control-allow-headers", "authorization,content-type");
            response.Headers.Add("access-control-allow-origin", configuration.CorsOrigins);
        }

        switch (error)
        {
            case UnauthorizedException _:
                response.StatusCode = (int)HttpStatusCode.Unauthorized;
                break;
            case NotFoundException notFoundException:
                response.ContentType = "text/plain";
                response.StatusCode = (int)HttpStatusCode.NotFound;
                await response.WriteAsync(notFoundException.Message);
                break;
            case LacosException lacosException:
                response.ContentType = "text/plain";
                response.StatusCode = (int)HttpStatusCode.BadRequest;
                await response.WriteAsync(lacosException.GetMessageRecursive());
                break;
            default:
                response.ContentType = "text/plain";
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
                await response.WriteAsync("Si è verificato un errore imprevisto.");
                break;
        }
    }
}