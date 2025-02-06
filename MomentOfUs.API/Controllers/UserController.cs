using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
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

        [HttpGet("UserInfo")]
        [Authorize]
        public async Task<IActionResult> GetUserInfo()
        {
            var userId = User.FindFirst("Id")?.Value;

            var userInfo = await _serviceManager.UserService.GetUserInfo(userId);

            return Ok(userInfo);
        }

        [HttpPatch("Update")]
        [Authorize]
        public async Task<IActionResult> UpdateUserInfo([FromBody] UserInfoDto userInfo)
        {
            var userId = User.FindFirst("Id")?.Value;

            await _serviceManager.UserService.UpdateUserInfo(userId,userInfo);

            return NoContent();
        }

        [HttpGet("Users")]
        [Authorize(Roles ="Admin")]
        public async Task<IActionResult> GetUsers()
        {
            var userList=await _serviceManager.UserService.GetUserInfoList();
            return Ok(userList);

        }
    }
}