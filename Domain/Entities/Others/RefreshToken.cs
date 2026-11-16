using Domain.Entities.BaseEntities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities.Others
{
    public class RefreshToken:BaseModel
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string TokenHash { get; set; } = null!;   // هش توکن، نه خود توکن
        public DateTime ExpiresAt { get; set; }
        public DateTime? RevokedAt { get; set; }          // با لاگ‌اوت پر می‌شود

        public bool IsActive => RevokedAt == null && ExpiresAt > DateTime.UtcNow;
    }
}
