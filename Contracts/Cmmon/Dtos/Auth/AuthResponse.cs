using System;
using System.Collections.Generic;
using System.Text;

namespace Contracts.Cmmon.Dtos.Auth
{
    public class AuthResponse
    {
        public string AccessToken { get; set; } = null!;
        public DateTime AccessTokenExpiresAt { get; set; }
        public string RefreshToken { get; set; } = null!;
    }
}
