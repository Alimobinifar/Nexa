using Application.Interfaces.ConnectionStringProtector;
using Application.Interfaces.Tenants;
using Domain.Entities.Tenants;
using Domain.Enums;
using Infrastructure.DataBase;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Tenancy
{
    public class TenantProvisioner : ITenantProvisioner
    {
        private readonly MasterDbContext _master;
        private readonly IConnectionStringProtector _protector;
        private readonly TenantDatabaseOptions _options;

        public TenantProvisioner(
            MasterDbContext master,
            IConnectionStringProtector protector,
            IOptions<TenantDatabaseOptions> options)
        {
            _master = master;
            _protector = protector;
            _options = options.Value;
        }

        public async Task<int> ProvisionAsync(string tenantName, CancellationToken cancellationToken = default)
        {
            // مرحله ۱: ثبت رکورد با وضعیت Provisioning (برای گرفتن Id)
            var tenant = new Tenant
            {
                Name = tenantName,
                ConnectionString = "pending",
                Status = TenantStatus.Provisioning
            };
            _master.Tenants.Add(tenant);
            await _master.SaveChangesAsync(cancellationToken);

            try
            {
                // مرحله ۲: ساخت Connection String با Id تننت
                var dbName = $"{_options.DatabaseNamePrefix}{tenant.Id}";
                var plainConnection = string.Format(_options.ConnectionTemplate, dbName);

                // مرحله ۳: ساخت دیتابیس و اجرای Migrationها
                var builder = new DbContextOptionsBuilder<TenantDbContext>();
                builder.UseSqlServer(plainConnection);

                await using (var tenantDb = new TenantDbContext(builder.Options))
                {
                    await tenantDb.Database.MigrateAsync(cancellationToken);
                }

                // مرحله ۴: ذخیره‌ی Connection String رمز‌شده و فعال‌سازی
                tenant.ConnectionString = _protector.Protect(plainConnection);
                tenant.Status = TenantStatus.Active;
                tenant.UpdatedAt = DateTime.UtcNow;
                await _master.SaveChangesAsync(cancellationToken);

                return tenant.Id;
            }
            catch
            {
                // پاک‌سازی: رکورد ناقص نباید بماند
                _master.Tenants.Remove(tenant);
                await _master.SaveChangesAsync(CancellationToken.None);
                throw;
            }
        }
    }
}
