using Lacos.GestioneCommesse.Application.CheckLists.Services;
using Lacos.GestioneCommesse.Application.Customers.Services;
using Lacos.GestioneCommesse.Application.Docs.Services;
using Lacos.GestioneCommesse.Application.MaintenancePriceLists.Services;
using Lacos.GestioneCommesse.Application.NotificationOperators.Service;
using Lacos.GestioneCommesse.Application.Operators.Services;
using Lacos.GestioneCommesse.Application.Products.Service;
using Lacos.GestioneCommesse.Application.Registry.Services;
using Lacos.GestioneCommesse.Application.Security;
using Lacos.GestioneCommesse.Application.Session;
using Lacos.GestioneCommesse.Application.Shared.Services;
using Lacos.GestioneCommesse.Application.Suppliers.Services;
using Lacos.GestioneCommesse.Application.Sync;
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
            .AddScoped<ISharedService, SharedService>()
            .AddScoped<IAccessTokenProvider, TAccessTokenProvider>()
            .AddScoped<ICustomerService, CustomerService>()
            .AddScoped<ISupplierService, SupplierService>()
            .AddScoped<IAddressService, AddressService>()
            .AddScoped<IActivityTypeService, ActivityTypeService>()
            .AddScoped<IProductTypeService, ProductTypeService>()
            .AddScoped<IVehicleService, VehicleService>()
            .AddScoped<IOperatorService, OperatorService>()
            .AddScoped<IProductService, ProductService>()
            .AddScoped<ICheckListService, CheckListService>()
            .AddScoped<IJobsService, JobsService>()
            .AddScoped<ISyncService, SyncService>()
            .AddScoped<IActivitiesService, ActivitiesService>()
            .AddScoped<IActivityProductsService, ActivityProductsService>()
            .AddScoped<IInterventionsService, InterventionsService>()
            .AddScoped<ITicketsService, TicketsService>()
            .AddScoped<IPurchaseOrdersService, PurchaseOrdersService>()
            .AddScoped<IMessagesService, MessagesService>()
            .AddScoped<INotificationOperatorService, NotificationOperatorService>()
            .AddScoped<IHelperTypeService, HelperTypeService>()
            .AddScoped<IHelperDocumentService, HelperDocumentService>()
            .AddScoped<IJobAccountingService, JobAccountingService>()
            .AddScoped<IAccountingTypeService, AccountingTypeService>()
            .AddScoped<IMaintenancePriceListService, MaintenancePriceListService>();

        services.AddHttpClient<INominatimService, NominatimService>();

        return services;
    }
}