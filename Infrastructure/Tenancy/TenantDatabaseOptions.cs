using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Tenancy
{
    public class TenantDatabaseOptions
    {
        public const string SectionName = "TenantDatabase";
        public string ConnectionTemplate { get; set; } = null!;
        public string DatabaseNamePrefix { get; set; } = "Nexa_Tenant_";
    }
}
