using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MomentOfUs.Application.Dtos.UserDto;
using MomentOfUs.Application.Service;
using MomentOfUs.Application.Service.Contracts;

namespace MomentOfUs.API.Controllers
{
    [ApiController]
    [Route("api/Users")]
    public class UserController : ControllerBase
    {
        private readonly IServiceManager _serviceManager;

        public UserController(IServiceManager serviceManager)
        {
            _serviceManager = serviceManager;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] UserRegisterDto userRegisterDto)
        {
            var token = await _serviceManager.AuthService.Register(userRegisterDto);
            return CreatedAtAction(nameof(Register), token);
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] UserLoginDto userLoginDto)
        {
            var token = await _serviceManager.AuthService.Login(userLoginDto);
            return CreatedAtAction(nameof(Login), token);
        }
        [HttpPost("Logout")]
        public async Task<IActionResult> Logout()
        {
            var userId = User.FindFirst("Id")?.Value;
            await _serviceManager.AuthService.Logout(userId);
            return CreatedAtAction(nameof(Logout), "User Succeffully Logged out");
        }
    }
}