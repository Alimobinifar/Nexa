using Application.Interfaces.Products;
using Application.Interfaces.SalesRegistration;
using Application.Interfaces.Suppliers;
using Application.Services.ProductServices;
using Application.Services.SaleRegistrationServices;
using Application.Services.SupplierServices;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

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

            return services;
        }
    }
}
