using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using MomentOfUs.Application.Service.Contracts;
using MomentOfUs.Domain.Models;

namespace MomentOfUs.Application.Service
{
    public class AuthService : IAuthService
    {
        private readonly ILogger _logger;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        public AuthService(ILogger logger, UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _logger= logger;
            _userManager= userManager;
            _signInManager= signInManager;
        }
        public Task<string> GenerateJwt()
        {
            _logger.LogInformation("Hello");
            throw new NotImplementedException();
        }

        public Task<string> Login()
        {
            throw new NotImplementedException();
        }

        public Task Logout()
        {
            throw new NotImplementedException();
        }

        public Task<string> Register()
        {
            throw new NotImplementedException();
        }
    }
}