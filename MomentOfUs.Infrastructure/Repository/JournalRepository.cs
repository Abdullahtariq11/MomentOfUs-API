using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MomentOfUs.Domain.Contracts;
using MomentOfUs.Domain.Models;

namespace MomentOfUs.Infrastructure.Repository
{
    public class JournalRepository : RepositoryBase<Journal>, IJournalRepository
    {
        public JournalRepository(RepositoryContext repositoryContext) : base(repositoryContext) { }

        public async Task<Journal?> GetByIdAsync(Guid journalId, bool trackChanges)
        {
            return await FindByCondition(j => j.Id == journalId, trackChanges)
                         .Include(j => j.journalEntries)
                         .Include(j => j.sharedJournals)
                         .SingleOrDefaultAsync();
        }

        public async Task<IEnumerable<Journal>> GetUserJournalsAsync(string userId, bool trackChanges)
        {
            return await FindByCondition(j => j.OwnerID == userId, trackChanges)
                         .Include(j => j.journalEntries)
                         .Include(j => j.sharedJournals)
                         .ToListAsync();
        }

        public async Task<bool> IsOwnerAsync(Guid journalId, string userId)
        {
            var journal = await FindByCondition(j => j.Id == journalId, trackChanges: false)
                                .SingleOrDefaultAsync();
            return journal != null && journal.OwnerID == userId;
        }

        public void Create(Journal journal) => base.Create(journal);
        public void Update(Journal journal) => base.Update(journal);
        public void Delete(Journal journal) => base.Delete(journal);
    }
}