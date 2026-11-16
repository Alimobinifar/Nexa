using Application.Interfaces.Tenants;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Tenancy
{
    public class ClaimsTenantProvider: ITenantProvider
    {
        private readonly IHttpContextAccessor _accessor;

        public ClaimsTenantProvider(IHttpContextAccessor accessor)
        {
            _accessor = accessor;
        }

        public int? TenantId
        {
            get
            {
                var value = _accessor.HttpContext?.User.FindFirst("tenant_id")?.Value;
                return int.TryParse(value, out var id) ? id : null;
            }
        }
    }
}
