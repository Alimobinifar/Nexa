using Application.Interfaces.IWarehouseInventory;
using Application.Interfaces.ProductAllocations;
using Application.Interfaces.ProductEntries;
using Application.Interfaces.Products;
using Application.Interfaces.SaleChannels;
using Application.Interfaces.SalesRegistration;
using Application.Interfaces.Suppliers;
using Application.Services.ProductChannelAllocationServices;
using Application.Services.ProductEntryServices;
using Application.Services.ProductServices;
using Application.Services.SaleRegistrationServices;
using Application.Services.SalesChannels;
using Application.Services.SupplierServices;
using Application.Services.WarehouseInventoryServices;
using Microsoft.Extensions.DependencyInjection;

namespace Application.Extensions
{
    public static class ApplicationServiceExtensions
    {
        public static IServiceCollection AddApplicationServices(
            this IServiceCollection services)
        {
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<IProductCategoryService, ProductCategoryService>();
            services.AddScoped<ISupplierService, SupplierService>();
            services.AddScoped<ISaleRegistrationService,SaleRegistrationService>();
            services.AddScoped<ISalesChannelService, SalesChannelService>();
            services.AddScoped<IWarehouseInventoryService, WarehouseInventoryService>();
            services.AddScoped<IProductChannelAllocationService, ProductChannelAllocationService>();
            services.AddScoped<IProductEntryService, ProductEntryService>();
            return services;
        }
    }
}
