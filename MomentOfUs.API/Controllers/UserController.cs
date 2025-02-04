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
            _serviceManager=serviceManager;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] UserRegisterDto userRegisterDto)
        {
            var token= await _serviceManager.AuthService.Register(userRegisterDto);
            return CreatedAtAction(nameof(Register), token);
        }
    }
}