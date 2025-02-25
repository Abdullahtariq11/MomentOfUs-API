using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MomentOfUs.Application.Dtos.SharedJournalDto;
using MomentOfUs.Application.Service.Contracts;
using MomentOfUs.Domain.Exceptions;
using MomentOfUs.Domain.Models;

namespace MomentOfUs.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/journals/{journalId}/shared")]  
    public class SharedJournalController : ControllerBase
    {
        private readonly IServiceManager _serviceManager;
        public SharedJournalController(IServiceManager serviceManager)
        {
            _serviceManager = serviceManager;
        }
        /// <summary>
        /// Share a journal with a user
        /// </summary>
        /// <param name="journalId"></param>
        /// <param name="userId"></param>
        /// <param name="permission"></param>
        /// <returns></returns>
        [HttpPost]  
        public async Task<IActionResult> ShareJournal(Guid journalId, [FromBody] SharedJournalCreateDto sharedJournalCreateDto)
        {
            try 
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (userId == null)
                {
                    return Unauthorized();
                }
                await _serviceManager.JournalService.ShareJournalAsync(journalId, userId, sharedJournalCreateDto.targetUserId, sharedJournalCreateDto.Permission);
                return Ok(new { message = "Journal shared successfully" });
            }
            catch (BadRequestException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        /// <summary>
        /// upadte the permission of a shared journal
        /// </summary>
        /// <param name="id"></param>
        /// <param name="permission"></param>
        /// <returns></returns>
        [HttpPut]
        public async Task<IActionResult> UpdatePermission(Guid journalId, [FromBody] SharedJournalUpdated sharedJournalUpdated)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (userId == null)
                {
                return Unauthorized();
            }
            await _serviceManager.JournalService.UpdateUserPermissionAsync(journalId, userId, sharedJournalUpdated.targetUserId, sharedJournalUpdated.Permission);
            return Ok(new { message = "Permission updated successfully" });
        }
        catch (BadRequestException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        }
        /// <summary>
        /// revoke user access to a shared journal
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        public async Task<IActionResult> RevokeAccess(Guid journalId, string targetUserId)  
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (userId == null)
                {
                    return Unauthorized();
                }
                await _serviceManager.JournalService.RevokeUserAccessAsync(journalId, userId, targetUserId);
                return NoContent();
            }
            catch (BadRequestException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }   
        /// <summary>
        /// get all users with access to a shared journal
        /// </summary>
        /// <param name="journalId"></param>
        /// <returns></returns>
        [HttpGet("users")]
        public async Task<IActionResult> GetAllUsersWithAccess()
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;  
                if (string.IsNullOrEmpty(userId))
                {
                    return BadRequest("Invalid user request.");
                }
                var users = await _serviceManager.JournalService.GetSharedUsersAsync(userId);
                return Ok(users);
            }
            catch (BadRequestException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }      

        
    }
}