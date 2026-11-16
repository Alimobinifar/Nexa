using System;
using System.Collections.Generic;
using System.Text;

namespace Contracts.Cmmon.Dtos.Auth
{
    public class RefreshTokenRequest
    {
        public string RefreshToken { get; set; } = null!;
    }
}
