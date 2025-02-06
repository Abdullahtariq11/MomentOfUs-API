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
   
        

        public ServiceManager(IServiceProvider serviceProvider)
        {
            _authService = new Lazy<IAuthService>(() => serviceProvider.GetRequiredService<IAuthService>());
            _userService = new Lazy<IUserService>(() => serviceProvider.GetRequiredService<IUserService>());
        }
        public IAuthService AuthService => _authService.Value;
        public IUserService UserService => _userService.Value;
    }
}