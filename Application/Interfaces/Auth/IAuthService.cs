using Contracts.Cmmon.Dtos.Auth;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces.Auth
{
    public interface IAuthService
    {
        Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken ct = default);
        Task<AuthResponse> RefreshAsync(string refreshToken, CancellationToken ct = default);
        Task LogoutAsync(string refreshToken, CancellationToken ct = default);
    }
}
