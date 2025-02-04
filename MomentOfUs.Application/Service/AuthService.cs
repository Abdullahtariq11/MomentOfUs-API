using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using MomentOfUs.Application.Dtos.UserDto;
using MomentOfUs.Application.Service.Contracts;
using MomentOfUs.Domain.Exceptions;
using MomentOfUs.Domain.Models;

namespace MomentOfUs.Application.Service
{
    public class AuthService : IAuthService
    {
        private readonly ILogger<AuthService> _logger;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IConfiguration _configuration;
        public AuthService(ILogger<AuthService> logger, UserManager<User> userManager, SignInManager<User> signInManager, IConfiguration configuration)
        {
            _logger = logger;
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
        }
        /// <summary>
        /// Generates JWT Token
        /// </summary>
        /// <returns></returns>
        public async Task<string> GenerateJwt(User user)
        {
            _logger.LogInformation("Generating JWT Token");

            //Retrieves JWT settings from appsettings.json.
            var jwtSettings = _configuration.GetSection("JwtSettings");
            //Encodes the secret key (Key) into a byte array,
            var secretKey = Encoding.UTF8.GetBytes(jwtSettings["Secret"]);
            //Create JWT Claims
            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName),                // Username
                new Claim(ClaimTypes.NameIdentifier, user.Id),            // UserID
                new Claim(ClaimTypes.GivenName, user.FirstName),          // First Name
                new Claim(ClaimTypes.Surname, user.LastName),             // Last Name
            };

            //Get user roles and add them as claim
            var roles = await _userManager.GetRolesAsync(user);
            foreach (var role in roles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, role));
            }

            //Converts the secret key into a SymmetricSecurityKey.
	        //This key will be used to digitally sign the JWT to prevent tampering.
            var authSigningKey = new SymmetricSecurityKey(secretKey);

            //Generate JWT Token
            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(jwtSettings["DurationInMinutes"])), // ✅ Use UTC time
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
            );

            return new JwtSecurityTokenHandler().WriteToken(token);

        }

        public Task<string> Login()
        {
            throw new NotImplementedException();
        }

        public Task Logout()
        {
            throw new NotImplementedException();
        }

        public async Task<string> Register(UserRegisterDto userRegisterDto)
        {
            _logger.LogInformation("Registering User");
            if (userRegisterDto == null)
            {
                throw new BadRequestException("Information not present or not in correct format.");
            }
            if(_userManager.FindByEmailAsync(userRegisterDto.Email)!=null)
            {
                throw new BadRequestException("User with same email exist");
            }
            var user= new User
            {
                UserName = userRegisterDto.Email,
                Email=userRegisterDto.Email,
                FirstName=userRegisterDto.FirstName,
                LastName=userRegisterDto.LastName,

            };
            var result= await _userManager.CreateAsync(user,userRegisterDto.Password);
            if(result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user,"Admin");

            }
            return await GenerateJwt(user); 
        }
    }
}