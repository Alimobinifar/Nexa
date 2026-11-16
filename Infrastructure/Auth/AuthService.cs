using Application.Interfaces.Auth;
using Contracts.Cmmon.Dtos.Auth;
using Domain.Entities.Others;
using Domain.Entities.Users;
using Domain.Enums;
using Infrastructure.DataBase;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Auth
{

    public class AuthService : IAuthService
    {
        private readonly MasterDbContext _db;
        private readonly UserManager<User> _userManager;
        private readonly IJwtTokenGenerator _jwt;
        private readonly JwtOptions _options;

        public AuthService(
            MasterDbContext db,
            UserManager<User> userManager,
            IJwtTokenGenerator jwt,
            IOptions<JwtOptions> options)
        {
            _db = db;
            _userManager = userManager;
            _jwt = jwt;
            _options = options.Value;
        }

        public async Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken ct = default)
        {
            var user = await _userManager.FindByNameAsync(request.UserName);

            // پیام یکسان برای «کاربر نیست» و «رمز غلط»، تا کسی نتونه نام‌های کاربری رو حدس بزنه
            if (user is null || !await _userManager.CheckPasswordAsync(user, request.Password))
                throw new UnauthorizedAccessException("نام کاربری یا رمز عبور اشتباه است.");

            if (user.IsDeleted || !user.IsActive || user.Status != UserStatus.Active)
                throw new UnauthorizedAccessException("حساب کاربری فعال نیست.");

            // کاربر عادی باید تننت فعال داشته باشد
            if (user.TenantId.HasValue)
            {
                var tenantActive = await _db.Tenants
                    .AnyAsync(t => t.Id == user.TenantId && t.Status == TenantStatus.Active, ct);
                if (!tenantActive)
                    throw new UnauthorizedAccessException("فضای کاری شما فعال نیست.");
            }

            user.LastLoginAt = DateTime.UtcNow;
            await _userManager.UpdateAsync(user);

            return await IssueTokensAsync(user, ct);
        }

        public async Task<AuthResponse> RefreshAsync(string refreshToken, CancellationToken ct = default)
        {
            var hash = _jwt.HashToken(refreshToken);
            var stored = await _db.RefreshTokens.FirstOrDefaultAsync(r => r.TokenHash == hash, ct);

            if (stored is null || !stored.IsActive)
                throw new UnauthorizedAccessException("توکن نامعتبر است.");

            var user = await _userManager.FindByIdAsync(stored.UserId.ToString());
            if (user is null || user.IsDeleted || !user.IsActive)
                throw new UnauthorizedAccessException("حساب کاربری فعال نیست.");

            // چرخش توکن: قدیمی باطل می‌شود و جدید صادر می‌شود
            stored.RevokedAt = DateTime.UtcNow;
            return await IssueTokensAsync(user, ct);
        }

        public async Task LogoutAsync(string refreshToken, CancellationToken ct = default)
        {
            var hash = _jwt.HashToken(refreshToken);
            var stored = await _db.RefreshTokens.FirstOrDefaultAsync(r => r.TokenHash == hash, ct);

            if (stored is not null && stored.RevokedAt is null)
            {
                stored.RevokedAt = DateTime.UtcNow;
                await _db.SaveChangesAsync(ct);
            }
        }

        private async Task<AuthResponse> IssueTokensAsync(User user, CancellationToken ct)
        {
            var roles = await _userManager.GetRolesAsync(user);
            var (accessToken, expiresAt) = _jwt.GenerateAccessToken(user, roles);

            var refreshToken = _jwt.GenerateRefreshToken();
            _db.RefreshTokens.Add(new RefreshToken
            {
                UserId = user.Id,
                TokenHash = _jwt.HashToken(refreshToken),
                ExpiresAt = DateTime.UtcNow.AddDays(_options.RefreshTokenDays)
            });
            await _db.SaveChangesAsync(ct);

            return new AuthResponse
            {
                AccessToken = accessToken,
                AccessTokenExpiresAt = expiresAt,
                RefreshToken = refreshToken
            };
        }
    }
}
