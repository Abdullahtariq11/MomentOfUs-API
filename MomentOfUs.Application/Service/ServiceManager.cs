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
   
        

        public ServiceManager(IServiceProvider serviceProvider)
        {
            _authService = new Lazy<IAuthService>(() => serviceProvider.GetRequiredService<IAuthService>());
            
        }
        public IAuthService AuthService => _authService.Value;
    }
}