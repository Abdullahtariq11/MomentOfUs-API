using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MomentOfUs.Application.Service.Contracts;

namespace MomentOfUs.Application.Service
{
    public class ServiceManager : IServiceManager
    {
        private readonly Lazy<IAuthService> _authService;
        private readonly Lazy<IUserService> _userService;
        private readonly Lazy<IJournalService> _journalService;



        public ServiceManager(IServiceProvider serviceProvider)
        {
            _authService = new Lazy<IAuthService>(() => serviceProvider.GetRequiredService<IAuthService>());
            _userService = new Lazy<IUserService>(() => serviceProvider.GetRequiredService<IUserService>());
            _journalService = new Lazy<IJournalService>(() => serviceProvider.GetRequiredService<IJournalService>());
        }
        public IAuthService AuthService => _authService.Value;
        public IUserService UserService => _userService.Value;

        public IJournalService JournalService => _journalService.Value;
    }
}