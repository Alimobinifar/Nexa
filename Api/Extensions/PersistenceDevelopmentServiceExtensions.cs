using Infrastructure.DataBase;
using Microsoft.EntityFrameworkCore;

namespace Api.Extensions
{
    public static class PersistenceDevelopmentServiceExtensions
    {

        public static IServiceCollection AddDevelopmentPersistenceServices(
                    this IServiceCollection services,
                    IConfiguration configuration)
        {
            return AddDbContext(
                services,
                configuration,
                "DevelopmentConnection");
        }

        public static IServiceCollection AddProductionPersistenceServices(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            return AddDbContext(
                services,
                configuration,
                "MainConnection");
        }

        private static IServiceCollection AddDbContext(
            IServiceCollection services,
            IConfiguration configuration,
            string connectionStringName)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString(connectionStringName)));

            return services;
        }
    }
}
