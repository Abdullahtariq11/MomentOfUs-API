using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MomentOfUs.Domain.Models;

namespace MomentOfUs.Domain.Contracts
{
    public interface ISharedJournalRepository
    {
        /// <summary>
        /// Gets a shared journal by its unique identifier
        /// </summary>
        Task<SharedJournal?> GetByIdAsync(Guid sharedJournalId, bool trackChanges);

        /// <summary>
        /// Gets all shared journal entries for a specific journal
        /// </summary>
        Task<IEnumerable<SharedJournal>> GetByJournalIdAsync(Guid journalId, bool trackChanges);

        /// <summary>
        /// Gets all shared journals owned by a user
        /// </summary>
        Task<IEnumerable<SharedJournal>> GetUserSharedJournalAsync(string userId, bool trackChanges);

        /// <summary>
        /// Gets the type of access a user has to a journal
        /// </summary>
        Task<PermissionLevel?> GetUserAccessAsync(Guid journalId, string userId);

        /// <summary>
        /// Creates a new shared journal entry
        /// </summary>
        Task CreateAsync(SharedJournal sharedJournal);

        /// <summary>
        /// Deletes a shared journal entry
        /// </summary>
        Task Delete(SharedJournal sharedJournal);

        /// <summary>
        /// Updates a shared journal entry
        /// </summary>
        Task Update(SharedJournal sharedJournal);
    }
}