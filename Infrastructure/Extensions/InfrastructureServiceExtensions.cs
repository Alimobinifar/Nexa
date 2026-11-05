using Application.Interfaces.Products;
using Application.Services.ProductServices;
using Domain.Interfaces.GenericInterfaces;
using Domain.Interfaces.UnitOfWork;
using Infrastructure.Repositories.GenericRepositories;
using Infrastructure.Repositories.UnitOfWorkRepositories;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Extensions
{
    public static class InfrastructureServiceExtensions
    {
        public static IServiceCollection AddInfrastructureServices(
              this IServiceCollection services)
        {
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped(
                    typeof(IGenericRepository<>),
                    typeof(GenericRepository<>));

            return services;
        }
    }
}
