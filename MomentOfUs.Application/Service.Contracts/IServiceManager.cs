using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MomentOfUs.Application.Service.Contracts
{
    public interface IServiceManager
    {
        public IAuthService AuthService { get; }
        public IUserService UserService { get; }
        public IJournalService JournalService { get;  }
    }
}