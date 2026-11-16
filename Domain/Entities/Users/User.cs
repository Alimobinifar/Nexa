using Domain.Entities.BaseEntities;
using Domain.Entities.Tenants;
using Domain.Enums;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
//using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.Users
{
    public class User : IdentityUser<int>
    {
        // ======== اطلاعات شخصی ========
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;

        // ======== اطلاعات حسابداری ========
        public decimal? UserCredit { get; set; } = 0;   
        // ======== وضعیت کاربر ========
        public UserStatus Status { get; set; } = UserStatus.Active;
        public DateTime? LastLoginAt { get; set; }
        public DateTime? LastPasswordChangeAt { get; set; }

        // ======== سئو و اطلاعات عمومی ========
        public string? AvatarUrl { get; set; }       
        public string? Bio { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; } = null;
        public bool IsDeleted { get; set; } = false;
        public bool IsActive { get; set; } = true;
        public int? TenantId { get; set; }     // برای SuperAdmin خالی (null) است
        public Tenant? Tenant { get; set; }
    }
}
