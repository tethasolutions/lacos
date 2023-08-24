using Lacos.GestioneCommesse.Application.CheckLists.Services;
using Lacos.GestioneCommesse.Application.Customers.Services;
using Lacos.GestioneCommesse.Application.Docs.Services;
using Lacos.GestioneCommesse.Application.Operators.Services;
using Lacos.GestioneCommesse.Application.Products.Service;
using Lacos.GestioneCommesse.Application.Registry.Services;
using Lacos.GestioneCommesse.Application.Security;
using Lacos.GestioneCommesse.Application.Session;
using Lacos.GestioneCommesse.Application.Vehicles.Services;
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
            .AddScoped<ICustomerService, CustomerService>()
            .AddScoped<IAddressService, AddressService>()
            .AddScoped<IActivityTypeService, ActivityTypeService>()
            .AddScoped<IProductTypeService, ProductTypeService>()
            .AddScoped<IVehicleService, VehicleService>()
            .AddScoped<IOperatorService, OperatorService>()
            .AddScoped<IProductService, ProductService>()
            .AddScoped<ICheckListService, CheckListService>()
            .AddScoped<IJobsService, JobsService>()
            .AddScoped<IActivitiesService, ActivitiesService>()
            .AddScoped<IActivityProductsService, ActivityProductsService>()
            .AddScoped<IInterventionsService, InterventionsService>();

        return services;
    }
}