using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MomentOfUs.Domain.Models;

namespace MomentOfUs.Domain.Contracts
{
    public interface IUserSharedJournalRepository : IRepositoryBase<UserSharedJournal>
    {
         Task<UserSharedJournal?> GetUserPermissionAsync(Guid sharedJournalId, string userId, bool trackChanges);
        Task<IEnumerable<UserSharedJournal>> GetUsersWithAccessAsync(Guid sharedJournalId, bool trackChanges);
        new Task CreateAsync(UserSharedJournal userSharedJournal);
        new Task Update(UserSharedJournal userSharedJournal);
        new Task Delete(UserSharedJournal userSharedJournal);
        Task<bool> UpdateUserPermissionAsync(Guid sharedJournalId, string userId, PermissionLevel newPermission);
        Task<bool> RevokeUserAccessAsync(Guid sharedJournalId, string userId);
        Task<bool> GrantUserAccessAsync(Guid sharedJournalId, string userId, PermissionLevel permissionLevel);
    }
}