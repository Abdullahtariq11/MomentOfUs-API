using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MomentOfUs.Domain.Contracts;
using MomentOfUs.Domain.Models;

namespace MomentOfUs.Infrastructure.Repository
{
    public class UserSharedJournalRepository : RepositoryBase<UserSharedJournal>, IUserSharedJournalRepository
    {
        public UserSharedJournalRepository(RepositoryContext repositoryContext) : base(repositoryContext)
        {

        }

        public async Task<UserSharedJournal?> GetUserPermissionAsync(Guid sharedJournalId, string userId, bool trackChanges)
        {
            return await base.FindByCondition(ush => ush.SharedJournalId == sharedJournalId && ush.UserId == userId, trackChanges)
            .Include(x => x.User)
            .Include(x => x.SharedJournal)
            .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<UserSharedJournal>> GetUsersWithAccessAsync(Guid sharedJournalId, bool trackChanges)
        {
            return await base.FindByCondition(ush => ush.SharedJournalId == sharedJournalId, trackChanges).ToListAsync();
        }

        public async Task<bool> GrantUserAccessAsync(Guid sharedJournalId, string userId, PermissionLevel permissionLevel)
        {
            // Check if the user already has access
            var existingUser = await base.FindByCondition(ush => ush.SharedJournalId == sharedJournalId && ush.UserId == userId && ush.PermissionLevel == permissionLevel, trackChanges: false)
            .AnyAsync();

            if (existingUser)
            {
                return false; // User already has access
            }
            // Create a new UserSharedJournal entry
            var shared = new UserSharedJournal
            {
                Id = Guid.NewGuid(),
                SharedJournalId = sharedJournalId,
                UserId = userId,
                PermissionLevel = permissionLevel

            };
            // Add the new access entry
            await CreateAsync(shared);
            return true;
        }

        public async Task<bool> RevokeUserAccessAsync(Guid sharedJournalId, string userId)
        {
            var existingUser = await base.FindByCondition(ush => ush.SharedJournalId == sharedJournalId && ush.UserId == userId, trackChanges: false).FirstOrDefaultAsync();
            if (existingUser == null)
                return false;
            Delete(existingUser);
            return true;
        }

        public async Task<bool> UpdateUserPermissionAsync(Guid sharedJournalId, string userId, PermissionLevel newPermission)
        {
            var existingUser = await base.FindByCondition(ush => ush.SharedJournalId == sharedJournalId && ush.UserId == userId, trackChanges: false).FirstOrDefaultAsync();
            if (existingUser == null)
                return false;
            existingUser.PermissionLevel = newPermission;
            Update(existingUser);
            return true;
        }

        async Task IUserSharedJournalRepository.CreateAsync(UserSharedJournal userSharedJournal)
        {
            await base.CreateAsync(userSharedJournal);
        }

        Task IUserSharedJournalRepository.Delete(UserSharedJournal userSharedJournal)
        {
            base.Delete(userSharedJournal);
            return Task.CompletedTask;
        }

        Task IUserSharedJournalRepository.Update(UserSharedJournal userSharedJournal)
        {
            base.Delete(userSharedJournal);
            return Task.CompletedTask;
        }
    }
}