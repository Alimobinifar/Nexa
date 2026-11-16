using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Auth
{
    public class SuperAdminSeedOptions
    {
        public const string SectionName = "SeedSuperAdmin";
        public string UserName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
    }
}
