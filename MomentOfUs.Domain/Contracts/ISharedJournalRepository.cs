using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MomentOfUs.Domain.Models;

namespace MomentOfUs.Domain.Contracts
{
    public interface ISharedJournalRepository : IRepositoryBase<SharedJournal>
    {
        Task<SharedJournal> GetByIdAsync(Guid id, bool trackChanges);
        Task<IEnumerable<SharedJournal>> GetUserSharedJournalAsync(string userId, bool trackChanges);
        Task<PermissionLevel?> GetUserAccessAsync(Guid journalId, string userId);

        Task CreateAsync(SharedJournal sharedJournal);
        Task Update(SharedJournal sharedJournal);
        Task Delete(SharedJournal sharedJournal);


    }
}