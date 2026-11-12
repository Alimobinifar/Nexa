using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces.Tenants
{
    public interface ITenantProvisioner
    {
        Task<int> ProvisionAsync(string tenantName, CancellationToken cancellationToken = default);
    }
}
