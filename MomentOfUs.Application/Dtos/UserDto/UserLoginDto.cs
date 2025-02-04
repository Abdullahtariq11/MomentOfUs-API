using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MomentOfUs.Application.Dtos.UserDto
{
    public record UserLoginDto(string Username, string Password, bool RememberMe);

}