using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces.Tenants
{
    public interface ITenantConnectionResolver
    {
        Task<string> GetConnectionStringAsync(CancellationToken cancellationToken = default);
    }
}
