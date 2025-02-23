using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MomentOfUs.Application.Dtos.SharedJournalDto;
using MomentOfUs.Application.Service.Contracts;
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
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return Unauthorized();
            }
            await _serviceManager.JournalService.ShareJournalAsync(journalId, userId, sharedJournalCreateDto.targetUserId, sharedJournalCreateDto.Permission);
            return Ok();
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
            
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return Unauthorized();
            }
            await _serviceManager.JournalService.UpdateUserPermissionAsync(journalId, userId, sharedJournalUpdated.targetUserId, sharedJournalUpdated.Permission);
            return Ok();
        }
        /// <summary>
        /// revoke user access to a shared journal
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        public async Task<IActionResult> RevokeAccess(Guid journalId, string targetUserId)  
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return Unauthorized();
            }
            await _serviceManager.JournalService.RevokeUserAccessAsync(journalId, userId, targetUserId);
            return NoContent();
        }   
        /// <summary>
        /// get all users with access to a shared journal
        /// </summary>
        /// <param name="journalId"></param>
        /// <returns></returns>
        [HttpGet("users")]
        public async Task<IActionResult> GetAllUsersWithAccess(Guid journalId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;  
            if (userId == null)
            {
                return Unauthorized();
            }
            var users = await _serviceManager.JournalService.GetSharedUsersAsync(journalId);
            return Ok(users);
        }      


     


        
        
    }
}