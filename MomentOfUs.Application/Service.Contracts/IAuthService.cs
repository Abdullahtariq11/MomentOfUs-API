using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MomentOfUs.Application.Service.Contracts
{
    public interface IAuthService
    {
         Task<string> Register();
         Task<string> Login();
         Task<string> GenerateJwt();
         Task Logout();
    }
}