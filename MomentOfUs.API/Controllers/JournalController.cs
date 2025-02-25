using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MomentOfUs.Application.Dtos.JournalDto;
using MomentOfUs.Application.Service.Contracts;

namespace MomentOfUs.API.Controllers
{
    [ApiController]
    [Route("api/Journals")]
    public class JournalController : ControllerBase
    {
        private readonly IServiceManager _serviceManager;

        public JournalController(IServiceManager serviceManager)
        {
            _serviceManager = serviceManager;
        }

        /// <summary>
        /// Create a new journal
        /// </summary>
        /// <param name="journalCreateDto"></param>
        /// <returns></returns>
        /// 
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateJournal([FromBody] JournalCreateDto journalCreateDto)
        {
            /// Get the user id from the token
            var userId= User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if(userId==null)
            {
                return Unauthorized();
            }
            var journalId= await _serviceManager.JournalService.CreateJournalAsync(userId,journalCreateDto.Title,journalCreateDto.PhotoUrl);
            return CreatedAtAction(nameof(GetJournalById), new {id=journalId},journalId);
            
        }
        /// <summary>
        /// Get a journal by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize]
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetJournalById(Guid id)
        {
            var userId= User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if(userId==null)
            {
                return Unauthorized();
            }
            var journal= await _serviceManager.JournalService.GetJournalByIdAsync(id,userId);
            if(journal==null)
            {
                return NotFound();
            }
            return Ok(journal);
        }
        /// <summary>
        /// Update a journal
        /// </summary>
        /// <param name="id"></param>
        /// <param name="journalUpdateDto"></param>
        /// <returns></returns>
        /// 
        [Authorize]
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdateJournal(Guid id, [FromBody] JournalUpdateDto journalUpdateDto)
        {
            var userId= User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if(userId==null)
            {
                return Unauthorized();
            }
            await _serviceManager.JournalService.UpdateJournalAsync(id,userId,journalUpdateDto.Title,journalUpdateDto.PhotoUrl);
            return NoContent();
        }
        /// <summary>
        /// Delete a journal
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize]
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteJournal(Guid id)
        {
            var userId= User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if(userId==null)
            {
                return Unauthorized();
            }
            await _serviceManager.JournalService.DeleteJournalAsync(id,userId);
            return NoContent();
        }
        /// <summary>
        /// Get all journals for a user
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAllJournals()
        {
            var userId= User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if(userId==null)
            {
                return Unauthorized();
            }
            var journals= await _serviceManager.JournalService.GetUserJournalsAsync(userId);
            return Ok(journals);
        }
        
    }
}