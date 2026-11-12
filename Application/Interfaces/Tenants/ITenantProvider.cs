using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces.Tenants
{
    public interface ITenantProvider
    {
        int? TenantId { get; }
    }
}
