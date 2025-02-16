using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MomentOfUs.Domain.Models;

namespace MomentOfUs.Domain.Contracts
{
    public interface IJournalEntryRepository : IRepositoryBase<JournalEntry>
    {
        Task<JournalEntry?> GetByIdAsync(Guid entryId, bool trackChanges);
        Task<IEnumerable<JournalEntry>> GetByJournalIdAsync(Guid journalId, bool trackChanges);
        Task CreateAsync(JournalEntry entry);
        Task Update(JournalEntry entry);
        Task Delete(JournalEntry entry);
    }
}