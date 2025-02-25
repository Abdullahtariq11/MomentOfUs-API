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
        /// <summary>
        /// Initializes a new instance of the UserSharedJournalRepository
        /// </summary>
        public UserSharedJournalRepository(RepositoryContext repositoryContext) : base(repositoryContext)
        {
        }

        /// <summary>
        /// Gets the permission level for a specific user and journal
        /// </summary>
        /// <param name="sharedJournalId">The ID of the shared journal</param>
        /// <param name="userId">The ID of the user</param>
        /// <param name="trackChanges">Whether to track entity changes</param>
        /// <returns>The UserSharedJournal entry if found, null otherwise</returns>
        public async Task<UserSharedJournal?> GetUserPermissionAsync(Guid sharedJournalId, string userId, bool trackChanges)
        {
            return await base.FindByCondition(ush => ush.SharedJournalId == sharedJournalId && ush.UserId == userId, trackChanges)
             .FirstOrDefaultAsync();
        }

        /// <summary>
        /// Gets all users who have access to a specific journal
        /// </summary>
        /// <param name="sharedJournalId">The ID of the shared journal</param>
        /// <param name="trackChanges">Whether to track entity changes</param>
        /// <returns>Collection of UserSharedJournal entries</returns>
        public async Task<IEnumerable<UserSharedJournal>> GetUsersWithAccessAsync(Guid sharedJournalId, bool trackChanges)
        {
            return await base.FindByCondition(ush => ush.SharedJournalId == sharedJournalId, trackChanges).ToListAsync();
        }

        /// <summary>
        /// Grants access to a journal for a specific user
        /// </summary>
        /// <param name="sharedJournalId">The ID of the shared journal</param>
        /// <param name="userId">The ID of the user to grant access to</param>
        /// <param name="permissionLevel">The level of permission to grant</param>
        /// <returns>True if access was granted, false if user already has access</returns>
        public async Task<bool> GrantUserAccessAsync(Guid sharedJournalId, string userId, PermissionLevel permissionLevel)
        {
            var existingUser = await base.FindByCondition(ush => ush.SharedJournalId == sharedJournalId && ush.UserId == userId && ush.PermissionLevel == permissionLevel, trackChanges: false)
            .AnyAsync();

            if (existingUser)
            {
                return false;
            }

            var shared = new UserSharedJournal
            {
                Id = Guid.NewGuid(),
                SharedJournalId = sharedJournalId,
                UserId = userId,
                PermissionLevel = permissionLevel
            };

            await CreateAsync(shared);
            return true;
        }

        /// <summary>
        /// Revokes a user's access to a journal
        /// </summary>
        /// <param name="sharedJournalId">The ID of the shared journal</param>
        /// <param name="userId">The ID of the user to revoke access from</param>
        /// <returns>True if access was revoked, false if user didn't have access</returns>
        public async Task<bool> RevokeUserAccessAsync(Guid sharedJournalId, string userId)
        {
            var existingUser = await base.FindByCondition(ush => ush.SharedJournalId == sharedJournalId && ush.UserId == userId, trackChanges: true).FirstOrDefaultAsync();
            if (existingUser == null)
                return false;
            Delete(existingUser);
            return true;
        }

        /// <summary>
        /// Updates a user's permission level for a journal
        /// </summary>
        /// <param name="sharedJournalId">The ID of the shared journal</param>
        /// <param name="userId">The ID of the user</param>
        /// <param name="newPermission">The new permission level to set</param>
        /// <returns>True if permission was updated, false if user doesn't have access</returns>
        public async Task<bool> UpdateUserPermissionAsync(Guid sharedJournalId, string userId, PermissionLevel newPermission)
        {
            var existingUser = await base.FindByCondition(ush => ush.SharedJournalId == sharedJournalId && ush.UserId == userId, trackChanges: false).FirstOrDefaultAsync();
            if (existingUser == null)
                return false;
            existingUser.PermissionLevel = newPermission;
            Update(existingUser);
            return true;
        }

        /// <summary>
        /// Creates a new UserSharedJournal entry
        /// </summary>
        /// <param name="userSharedJournal">The UserSharedJournal entry to create</param>
        async Task IUserSharedJournalRepository.CreateAsync(UserSharedJournal userSharedJournal)
        {
            await base.CreateAsync(userSharedJournal);
        }

        /// <summary>
        /// Deletes a UserSharedJournal entry
        /// </summary>
        /// <param name="userSharedJournal">The UserSharedJournal entry to delete</param>
        Task IUserSharedJournalRepository.Delete(UserSharedJournal userSharedJournal)
        {
            base.Delete(userSharedJournal);
            return Task.CompletedTask;
        }

        /// <summary>
        /// Updates a UserSharedJournal entry
        /// </summary>
        /// <param name="userSharedJournal">The UserSharedJournal entry to update</param>
        Task IUserSharedJournalRepository.Update(UserSharedJournal userSharedJournal)
        {
            base.Update(userSharedJournal);
            return Task.CompletedTask;
        }
    }
}