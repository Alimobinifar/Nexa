using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Enums
{
    public enum LogActionType
    {
        Create = 1,
        Update = 2,
        Delete = 3,
        Login = 4,
        Logout = 5,
        View = 6,
        Download = 7,
        Upload = 8,
        Export = 9,
        Import = 10,
        Comment = 11,
        Other = 99,

    }
}
