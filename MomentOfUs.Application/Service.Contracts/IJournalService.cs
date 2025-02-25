using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MomentOfUs.Domain.Models;

namespace MomentOfUs.Application.Service.Contracts
{
    public interface IJournalService
    {
        // Journal operations
        Task<Guid> CreateJournalAsync(string userId, string title, string? photoUrl);
        Task<IEnumerable<Journal>> GetUserJournalsAsync(string userId);
        Task<Journal?> GetJournalByIdAsync(Guid journalId, string userId);
        Task UpdateJournalAsync(Guid journalId, string userId, string title, string? photoUrl);
        Task DeleteJournalAsync(Guid journalId, string userId);

        // Journal entry operations
        Task<Guid> AddJournalEntryAsync(Guid journalId, string userId, string content, JournalEntry.MoodType mood);
        Task<IEnumerable<JournalEntry>> GetJournalEntriesAsync(Guid journalId, string userId);
        Task<JournalEntry?> GetJournalEntryByIdAsync(Guid journalEntryId, Guid journalId, string userId);
        Task UpdateJournalEntryAsync(Guid journalId, Guid entryId, string userId, string content, JournalEntry.MoodType mood);
        Task DeleteJournalEntryAsync(Guid journalEntryId, string userId);

        // Shared journal operations
        Task ShareJournalAsync(Guid journalId, string ownerId, string targetUserId, PermissionLevel permission);
        Task RevokeUserAccessAsync(Guid journalId, string ownerId, string targetUserId);
        Task<IEnumerable<UserSharedJournal>> GetSharedUsersAsync(string userId);
        Task UpdateUserPermissionAsync(Guid journalId, string ownerId, string targetUserId, PermissionLevel newPermission);
        Task<IEnumerable<SharedJournal>> GetSharedJournalsForUserAsync(string userId);
    }
}