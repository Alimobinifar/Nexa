using Application.Interfaces.Tenants;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Tenancy
{

    public class HeaderTenantProvider : ITenantProvider
    {
        private readonly IHttpContextAccessor _accessor;

        public HeaderTenantProvider(IHttpContextAccessor accessor)
        {
            _accessor = accessor;
        }

        public int? TenantId
        {
            get
            {
                var value = _accessor.HttpContext?.Request.Headers["X-Tenant-Id"].FirstOrDefault();
                return int.TryParse(value, out var id) ? id : null;
            }
        }
    }
}
