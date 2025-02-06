using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MomentOfUs.Application.Dtos.UserDto;
using MomentOfUs.Domain.Models;

namespace MomentOfUs.Application.Service.Contracts
{
    public interface IUserService
    {
        Task<UserInfoDto> GetUserInfo(string userId);
        Task UpdateUserInfo(string userId,UserInfoDto userInfoDto);
        Task<List<UserInfoDto>> GetUserInfoList();
    }
}