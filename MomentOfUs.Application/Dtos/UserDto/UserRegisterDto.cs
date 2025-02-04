using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MomentOfUs.Application.Dtos.UserDto
{
    public record UserRegisterDto(string FirstName, string LastName, string Email, string Password);

}