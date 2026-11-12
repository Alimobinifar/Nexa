using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces.ConnectionStringProtector
{
    public interface IConnectionStringProtector
    {
        string Protect(string plainConnectionString);
        string Unprotect(string protectedConnectionString);
    }
}
