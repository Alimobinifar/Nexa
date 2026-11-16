using Contracts.Cmmon.Dtos.User;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces.Users
{
    public interface IUserService
    {
        // فقط SuperAdmin: ساخت تننت + دیتابیس + کاربر. شناسه‌ی کاربر را برمی‌گرداند
        Task<int> CreateUserAsync(CreateUserDto request, CancellationToken ct = default);
    }
}
