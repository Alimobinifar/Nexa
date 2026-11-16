using Application.Interfaces.Tenants;
using Application.Interfaces.Users;
using Contracts.Cmmon.Dtos.User;
using Domain.Entities.Users;
using Infrastructure.DataBase;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Auth
{
    public class UserService : IUserService
    {
        private readonly UserManager<User> _userManager;
        private readonly ITenantProvisioner _provisioner;
        private readonly MasterDbContext _db;

        public UserService(
            UserManager<User> userManager,
            ITenantProvisioner provisioner,
            MasterDbContext db)
        {
            _userManager = userManager;
            _provisioner = provisioner;
            _db = db;
        }

        public async Task<int> CreateUserAsync(CreateUserDto request, CancellationToken ct = default)
        {
            // ۱) قبل از ساخت دیتابیس (کار سنگین)، یکتا بودن نام کاربری را چک می‌کنیم
            if (await _userManager.FindByNameAsync(request.UserName) is not null)
                throw new InvalidOperationException("این نام کاربری قبلاً استفاده شده است.");

            // ۲) ساخت تننت و دیتابیس
            var tenantId = await _provisioner.ProvisionAsync(request.TenantName, ct);

            // ۳) ساخت کاربر
            var user = new User
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                UserName = request.UserName,
                Email = request.Email,
                TenantId = tenantId
            };

            var result = await _userManager.CreateAsync(user, request.Password);
            if (!result.Succeeded)
            {
                // اگر کاربر ساخته نشد، تننت ناقص را پاک می‌کنیم (Rollback دستی)
                var tenant = await _db.Tenants.FindAsync(new object[] { tenantId }, ct);
                if (tenant is not null)
                {
                    tenant.IsDeleted = true;
                    tenant.Status = Domain.Enums.TenantStatus.Suspended;
                    await _db.SaveChangesAsync(ct);
                }

                throw new InvalidOperationException(
                    string.Join(" | ", result.Errors.Select(e => e.Description)));
            }

            await _userManager.AddToRoleAsync(user, "User");
            return user.Id;
        }
    }
}
