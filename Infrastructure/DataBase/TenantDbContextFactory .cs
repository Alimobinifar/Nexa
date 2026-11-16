using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.DataBase
{
    public class TenantDbContextFactory : IDesignTimeDbContextFactory<TenantDbContext>
    {
        public TenantDbContext CreateDbContext(string[] args)
        {
            var options = new DbContextOptionsBuilder<TenantDbContext>()
                .UseSqlServer("Server=ALIMOBINIFAR\\MOBINIFSQLSERVER;Database=Nexa_Tenant_Design;Integrated Security=True;TrustServerCertificate=True")
                .Options;

            return new TenantDbContext(options);
        }
    }
}
