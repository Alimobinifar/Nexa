using Domain.Entities.Users;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces.Auth
{
    public interface IJwtTokenGenerator
    {
        (string Token, DateTime ExpiresAt) GenerateAccessToken(User user, IList<string> roles);
        string GenerateRefreshToken();
        string HashToken(string token);
    }
}
