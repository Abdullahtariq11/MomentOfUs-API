using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using MomentOfUs.Application.Service.Contracts;

namespace MomentOfUs.Application.Service
{
    public class ServiceManager : IServiceManager
    {
        private readonly Lazy<IAuthService> _authService;
        private readonly ILogger _logger;
        

        public ServiceManager(IAuthService authService, ILogger logger)
        {
            _logger= logger;
            _authService = new Lazy<IAuthService>(()=> authService);
            
        }
        public IAuthService AuthService => _authService.Value;
    }
}