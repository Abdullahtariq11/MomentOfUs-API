using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MomentOfUs.Application.Service.Contracts;
using MomentOfUs.Application.Dtos.JournalEntryDto;
using Microsoft.AspNetCore.Authorization;
namespace MomentOfUs.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/Journals/{journalId}/entries")]
    public class JournalEntryController : ControllerBase
    {
        private readonly IServiceManager _serviceManager;

        public JournalEntryController(IServiceManager serviceManager)
        {
            _serviceManager = serviceManager;
        }

        /// <summary>
        /// Get all entries for a journal
        /// </summary>
        /// <param name="journalId"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetAllEntries(Guid journalId)
        {
            var userId= User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if(userId==null)
            {
                return Unauthorized();
            }
            var entries = await _serviceManager.JournalService.GetJournalEntriesAsync(journalId,userId);
            return Ok(entries);
        }

        /// <summary>
        /// Get an entry by id
        /// </summary>
        /// <param name="journalId"></param>
        /// <param name="entryId"></param>
        /// <returns></returns>
        [HttpGet("{entryId:guid}")]
        public async Task<IActionResult> GetEntryById(Guid journalId,Guid entryId)
        {
            var userId= User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if(userId==null)
            {
                return Unauthorized();
            }
            var entry= await _serviceManager.JournalService.GetJournalEntryByIdAsync(journalId,entryId,userId);
            if(entry==null)
            {
                return NotFound();
            }
            return Ok(entry);
        }

        /// <summary>
        /// Create a new entry
        /// </summary>
        /// <param name="journalId">The ID of the journal</param>
        /// <param name="entry">The entry details to create</param>
        /// <returns>The created entry ID</returns>
        [HttpPost]
        public async Task<IActionResult> CreateEntry(Guid journalId, JournalEntryCreateDto entry)
        {   
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return Unauthorized();
            }
            var entryId = await _serviceManager.JournalService.AddJournalEntryAsync(journalId, userId, entry.Content, entry.Mood);
            return CreatedAtAction(nameof(GetEntryById), new { journalId, entryId }, entryId);
        }

        /// <summary>
        /// Update an entry
        /// </summary>
        /// <param name="journalId">The ID of the journal</param>
        /// <param name="entryId">The ID of the entry to update</param>
        /// <param name="entry">The updated entry details</param>
        /// <returns>No content if successful</returns>
        [HttpPut("{entryId:guid}")]
        public async Task<IActionResult> UpdateEntry(Guid journalId, Guid entryId, JournalEntryUpdateDto entry)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return Unauthorized();
            }
            await _serviceManager.JournalService.UpdateJournalEntryAsync(journalId, entryId, userId, entry.Content, entry.Mood);
            return NoContent();
        }

        /// <summary>
        /// Delete an entry
        /// </summary>
        /// <param name="journalId"></param>
        /// <param name="entryId"></param>
        /// <returns></returns>
        [HttpDelete("{entryId:guid}")]
        public async Task<IActionResult> DeleteEntry(Guid journalId, Guid entryId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return Unauthorized();
            }
            await _serviceManager.JournalService.DeleteJournalEntryAsync(entryId, userId);
            return NoContent();
        }

                            
    }
}