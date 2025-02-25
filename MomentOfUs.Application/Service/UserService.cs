using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MomentOfUs.Application.Dtos.UserDto;
using MomentOfUs.Application.Service.Contracts;
using MomentOfUs.Domain.Exceptions;
using MomentOfUs.Domain.Models;

namespace MomentOfUs.Application.Service
{
    public class UserService : IUserService
    {
        private readonly ILogger<UserService> _logger;
        private readonly UserManager<User> _userManager;

        public UserService(ILogger<UserService> logger, UserManager<User> userManager)
        {
            _logger = logger;
            _userManager = userManager;
        }
        public async Task<UserInfoDto> GetUserInfo(string userId)
        {
            _logger.LogInformation("Retrieving user inromation with userId:{userId}", userId);
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                throw new NotFoundException($"User with id: {userId} not found", userId);
            }
            UserInfoDto userInfo = new(user.Id,user.FirstName, user.LastName, user.Email);
            return userInfo;
        }

        public async Task<List<UserInfoDto>> GetUserInfoList()
        {
            _logger.LogInformation("Retrieving user List");
            var users = await _userManager.Users.ToListAsync();
            if (users == null)
            {
                throw new BadRequestException("User list is empty");
            }
            List<UserInfoDto> userInfoDtos = new List<UserInfoDto>();
            foreach (var user in users)
            {
                var userInfo = new UserInfoDto(user.Id,user.FirstName, user.LastName, user.Email);
                if (userInfo == null)
                {
                    throw new BadRequestException($"User information empty for userId :{user.Id}");
                }
                userInfoDtos.Add(userInfo);
            }
            return userInfoDtos;
        }

        public async Task UpdateUserInfo(string userId, UserInfoDto userInfoDto)
        {
            _logger.LogInformation("Updating user information for user with id:{userId}", userId);

            if (userInfoDto == null)
            {
                throw new BadRequestException("UserInfo is an empty object");
            }

            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                throw new NotFoundException($"User does not exist with userId:{userId}");
            }

            user.FirstName = userInfoDto.FirstName;
            user.LastName = userInfoDto.LastName;
            user.Email = userInfoDto.Email;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                throw new BadRequestException("Unable to update user information");
            }

            _logger.LogInformation("User Updated Successfully!");

        }

        public async Task<bool> UserExist(string userId)
        {
            _logger.LogInformation($"Checking if user with id: {userId} exists");

            return await _userManager.Users.AnyAsync(u => u.Id == userId);
        }
    }
}