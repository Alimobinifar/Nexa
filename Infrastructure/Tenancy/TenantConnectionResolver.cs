using Application.Interfaces.ConnectionStringProtector;
using Application.Interfaces.Tenants;
using Infrastructure.DataBase;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Tenancy
{
    public class TenantConnectionResolver : ITenantConnectionResolver
    {
        private readonly ITenantProvider _tenantProvider;
        private readonly MasterDbContext _master;
        private readonly IConnectionStringProtector _protector;

        public TenantConnectionResolver(
            ITenantProvider tenantProvider,
            MasterDbContext master,
            IConnectionStringProtector protector)
        {
            _tenantProvider = tenantProvider;
            _master = master;
            _protector = protector;
        }

        public async Task<string> GetConnectionStringAsync(CancellationToken cancellationToken = default)
        {
            var tenantId = _tenantProvider.TenantId
                ?? throw new InvalidOperationException("تننت برای این درخواست مشخص نشده است.");

            var tenant = await _master.Tenants
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Id == tenantId, cancellationToken)
                ?? throw new InvalidOperationException($"تننت {tenantId} پیدا نشد.");

            if (tenant.Status != Domain.Enums.TenantStatus.Active)
                throw new InvalidOperationException($"تننت {tenantId} فعال نیست.");

            return _protector.Unprotect(tenant.ConnectionString);
        }
    }
}
