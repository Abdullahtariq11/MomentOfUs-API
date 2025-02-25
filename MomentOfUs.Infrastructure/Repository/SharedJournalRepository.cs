using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MomentOfUs.Domain.Contracts;
using MomentOfUs.Domain.Models;

namespace MomentOfUs.Infrastructure.Repository
{
    public class SharedJournalRepository : RepositoryBase<SharedJournal>, ISharedJournalRepository
    {
        public SharedJournalRepository(RepositoryContext repositoryContext) : base(repositoryContext)
        {
        }

        /// <summary>
        /// Gets a shared journal by its unique identifier
        /// </summary>
        /// <param name="sharedJournalId">The ID of the shared journal entry</param>
        /// <param name="trackChanges">Whether to track entity changes</param>
        /// <returns>The SharedJournal if found, null otherwise</returns>
        public async Task<SharedJournal?> GetByIdAsync(Guid sharedJournalId, bool trackChanges)
        {
            return await FindByCondition(sj => sj.Id == sharedJournalId, trackChanges)
                .Include(sj => sj.SharedWith)
                .SingleOrDefaultAsync();
        }

        /// <summary>
        /// Gets all shared journal entries for a specific journal
        /// </summary>
        /// <param name="journalId">The ID of the journal</param>
        /// <param name="trackChanges">Whether to track entity changes</param>
        /// <returns>List of SharedJournal entries for the specified journal</returns>
        public async Task<IEnumerable<SharedJournal>> GetByJournalIdAsync(Guid journalId, bool trackChanges)
        {
            return await FindByCondition(sj => sj.JournalId == journalId, trackChanges)
                .Include(sj => sj.SharedWith)
                .ToListAsync();
        }

        /// <summary>
        /// Get all shared journals owned by a user
        /// </summary>
        public async Task<IEnumerable<SharedJournal>> GetUserSharedJournalAsync(string userId, bool trackChanges)
        {
            return await FindByCondition(sj => sj.OwnerId == userId, trackChanges)
                .Include(sj => sj.SharedWith) // Include shared users
                .ToListAsync();
        }

        /// <summary>
        /// Get the type of access a user has to a journal (Owner, Edit, View, or None).
        /// </summary>
        public async Task<PermissionLevel?> GetUserAccessAsync(Guid journalId, string userId)
        {
             var sharedJournal = await FindByCondition(sj => sj.JournalId == journalId, trackChanges: false)
            .Include(sj => sj.SharedWith)
            .ThenInclude(uj => uj.User) // Ensure user details are loaded
            .FirstOrDefaultAsync(); 

            if (sharedJournal == null)
            {
                return null; // Journal not found or not shared
            }

            if (sharedJournal.OwnerId == userId)
            {
                return PermissionLevel.Edit; // Owner has full edit rights
            }

            var userPermission = sharedJournal.SharedWith.FirstOrDefault(uj => uj.UserId == userId);

            return userPermission?.PermissionLevel ?? null; // Explicitly return null if user has no access
        }

        /// <summary>
        /// Create a Shared Journal entry
        /// </summary>
        public async Task CreateAsync(SharedJournal sharedJournal)
        {
            await base.CreateAsync(sharedJournal);
        }

        /// <summary>
        /// Delete a Shared Journal entry
        /// </summary>
        public Task Delete(SharedJournal sharedJournal)
        {
            base.Delete(sharedJournal);
            return Task.CompletedTask;
        }

        /// <summary>
        /// Updates a shared journal
        /// </summary>
        /// <param name="sharedJournal"></param>
        /// <returns></returns>
        Task ISharedJournalRepository.Update(SharedJournal sharedJournal)
        {
            base.Update(sharedJournal);
            return Task.CompletedTask;
        }
    }
}