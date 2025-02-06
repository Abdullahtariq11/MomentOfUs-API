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

            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName),                // Username
                new Claim(ClaimTypes.NameIdentifier, user.Id),            // UserID
                 new Claim("Id", user.Id),
                new Claim("FirstName", user.FirstName),                   // First Name
                new Claim("LastName", user.LastName),                      // Last Name
                new Claim("SecurityStamp", user.SecurityStamp)
            };


            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:Secret"]));
            var token = new JwtSecurityToken(
                issuer: _configuration["JwtSettings:Issuer"],
                audience: _configuration["JwtSettings:Audience"],
                expires: DateTime.Now.AddMinutes(Convert.ToDouble(_configuration["JwtSettings:DurationInMinutes"])),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
            );

            return new JwtSecurityTokenHandler().WriteToken(token);

        }

        public async Task<string> Login(UserLoginDto userLoginDto)
        {
            _logger.LogInformation("Logging in user:{user}", userLoginDto.Username);
            var result = await _signInManager.PasswordSignInAsync(userLoginDto.Username, userLoginDto.Password, userLoginDto.RememberMe, lockoutOnFailure: false);
            if (!result.Succeeded)
            {
                throw new BadRequestException("Inavlid Login Attempt");
            }
            var user = await _userManager.FindByNameAsync(userLoginDto.Username);
            return await GenerateJwt(user);

        }

        public async Task Logout(string userId)
        {
            _logger.LogInformation("logging user out with Id:{id}", userId);
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                throw new BadRequestException($"Unable to load user with id {userId}");
            }
            await _userManager.UpdateSecurityStampAsync(user);
            await _signInManager.SignOutAsync();

        }

        public async Task<string> Register(UserRegisterDto userRegisterDto)
        {
            _logger.LogInformation("Registering User");
            if (userRegisterDto == null)
            {
                throw new BadRequestException("Information not present or not in correct format.");
            }
            if (_userManager.FindByEmailAsync(userRegisterDto.Email) != null)
            {
                throw new BadRequestException("User with same email exist");
            }
            var user = new User
            {
                UserName = userRegisterDto.Email,
                Email = userRegisterDto.Email,
                FirstName = userRegisterDto.FirstName,
                LastName = userRegisterDto.LastName,

            };
            var result = await _userManager.CreateAsync(user, userRegisterDto.Password);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "Admin");

            }
            return await GenerateJwt(user);
        }
    }
}