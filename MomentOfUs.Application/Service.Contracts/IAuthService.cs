using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MomentOfUs.Application.Dtos.UserDto;
using MomentOfUs.Domain.Models;

namespace MomentOfUs.Application.Service.Contracts
{
    public interface IAuthService
    {
         Task<string> Register(UserRegisterDto userRegisterDto);
         Task<string> Login(UserLoginDto userLoginDto);
         Task<string> GenerateJwt(User user);
         Task Logout(string userId);
    }
}