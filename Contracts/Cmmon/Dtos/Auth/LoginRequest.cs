using System;
using System.Collections.Generic;
using System.Text;

namespace Contracts.Cmmon.Dtos.Auth
{
    public class LoginRequest
    {
        public string UserName { get; set; } = null!;
        public string Password { get; set; } = null!;
    }

}
