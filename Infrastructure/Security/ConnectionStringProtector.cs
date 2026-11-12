using Application.Interfaces.ConnectionStringProtector;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Security
{
    public class ConnectionStringProtector : IConnectionStringProtector
    {
        private readonly IDataProtector _protector;

        public ConnectionStringProtector(IDataProtectionProvider provider)
        {
            _protector = provider.CreateProtector("Nexa.TenantConnectionString.v1");
        }

        public string Protect(string plainConnectionString)
            => _protector.Protect(plainConnectionString);

        public string Unprotect(string protectedConnectionString)
            => _protector.Unprotect(protectedConnectionString);
    }
}
