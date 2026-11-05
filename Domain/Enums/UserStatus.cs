using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Enums
{
    public enum UserStatus
    {
        Active = 1,
        Inactive = 2,
        Banned = 3,
        Deleted = 4
    }

    public enum UserRoleType
    {
        SuperAdmin = 0,
        Admin = 1,
        Customer = 2,
    }
}
