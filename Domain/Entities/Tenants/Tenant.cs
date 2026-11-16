using Domain.Entities.BaseEntities;
using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities.Tenants
{
    public class Tenant:BaseModel
    {
        public string Name { get; set; } = null!; // This is a sample name 
        public string ConnectionString { get; set; } = null!;
        public TenantStatus Status { get; set; } = TenantStatus.Provisioning;
    }
}
