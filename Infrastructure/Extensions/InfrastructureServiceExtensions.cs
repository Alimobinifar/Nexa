using Application.Interfaces.Auth;
using Application.Interfaces.ConnectionStringProtector;
using Application.Interfaces.Products;
using Application.Interfaces.Tenants;
using Application.Interfaces.Users;
using Application.Services.ProductServices;
using Domain.Entities.Users;
using Domain.Interfaces.GenericInterfaces;
using Domain.Interfaces.UnitOfWork;
using Infrastructure.Auth;
using Infrastructure.DataBase;
using Infrastructure.Repositories.GenericRepositories;
using Infrastructure.Repositories.UnitOfWorkRepositories;
using Infrastructure.Security;
using Infrastructure.Tenancy;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Extensions
{
    public static class InfrastructureServiceExtensions
    {
        public static IServiceCollection AddInfrastructureServices(
              this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped(
                    typeof(IGenericRepository<>),
                    typeof(GenericRepository<>));
            services.AddDbContext<MasterDbContext>(options =>
          options.UseSqlServer(configuration.GetConnectionString("MasterDb")));

            // ۲) تنظیمات تننت از appsettings
            services.Configure<TenantDatabaseOptions>(
                configuration.GetSection(TenantDatabaseOptions.SectionName));

            // ۳) رمزنگاری: کلیدها در یک پوشه‌ی ثابت ذخیره می‌شن
            services.AddDataProtection()
                .SetApplicationName("Nexa")
                .PersistKeysToFileSystem(new DirectoryInfo(
                    Path.Combine(AppContext.BaseDirectory, "keys")));

            services.AddScoped<IConnectionStringProtector, ConnectionStringProtector>();

            // ۴) سرویس‌های تننت
            services.AddHttpContextAccessor();
            services.AddScoped<ITenantProvider, ClaimsTenantProvider>();
            services.AddScoped<ITenantConnectionResolver, TenantConnectionResolver>();
            services.AddScoped<ITenantProvisioner, TenantProvisioner>();

            services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));
            services.Configure<SuperAdminSeedOptions>(configuration.GetSection(SuperAdminSeedOptions.SectionName));

            services.AddIdentityCore<User>(options =>
            {
                options.Password.RequiredLength = 8;
                options.User.RequireUniqueEmail = true;
            })
                .AddRoles<IdentityRole<int>>()
                .AddEntityFrameworkStores<MasterDbContext>();

            services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IUserService, UserService>();

            // ۵) دیتابیس تننت: Connection String هر درخواست جداگانه تعیین می‌شه
            services.AddDbContext<TenantDbContext>((sp, options) =>
            {
                var tenantProvider = sp.GetRequiredService<ITenantProvider>();

                // اگر تننتی مشخص نیست (مثلاً موقع اجرای ابزار Migration)، Connection String نمونه
                var connectionString = tenantProvider.TenantId is null
                    ? "Server=ALIMOBINIFAR\\MOBINIFSQLSERVER;Database=Nexa_Tenant_Design;Integrated Security=True;TrustServerCertificate=True"
                    : sp.GetRequiredService<ITenantConnectionResolver>()
                        .GetConnectionStringAsync().GetAwaiter().GetResult();

                options.UseSqlServer(connectionString);
            });

            return services;
        }
    }
}
