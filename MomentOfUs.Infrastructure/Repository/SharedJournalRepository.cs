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
        /// Get shared journal by its Id
        /// </summary>
        public async Task<SharedJournal?> GetByIdAsync(Guid id, bool trackChanges)
        {
            return await FindByCondition(sj => sj.Id == id, trackChanges)
                .Include(sj => sj.SharedWith) // Include shared users
                .SingleOrDefaultAsync();
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
                .SingleOrDefaultAsync();

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
        public Task Create(SharedJournal sharedJournal)
        {
            base.Create(sharedJournal);
            return Task.CompletedTask;
        }

        /// <summary>
        /// Delete a Shared Journal entry
        /// </summary>
        public Task Delete(SharedJournal sharedJournal)
        {
            base.Delete(sharedJournal);
            return Task.CompletedTask;
        }
    }
}