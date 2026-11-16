using Domain.Entities.Users;
using Infrastructure.DataBase;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Auth
{
    public static class IdentitySeeder
    {
        public static async Task SeedAsync(IServiceProvider services)
        {
            using var scope = services.CreateScope();
            var sp = scope.ServiceProvider;

            var db = sp.GetRequiredService<MasterDbContext>();
            await db.Database.MigrateAsync();   // دیتابیس مستر همیشه به‌روز می‌شود

            var roleManager = sp.GetRequiredService<RoleManager<IdentityRole<int>>>();
            foreach (var role in new[] { "SuperAdmin", "User" })
            {
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole<int>(role));
            }

            var userManager = sp.GetRequiredService<UserManager<User>>();
            var seed = sp.GetRequiredService<IOptions<SuperAdminSeedOptions>>().Value;

            if (await userManager.FindByNameAsync(seed.UserName) is null)
            {
                var admin = new User
                {
                    FirstName = "Super",
                    LastName = "Admin",
                    UserName = seed.UserName,
                    Email = seed.Email,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(admin, seed.Password);
                if (result.Succeeded)
                    await userManager.AddToRoleAsync(admin, "SuperAdmin");
            }
        }
    }
}
