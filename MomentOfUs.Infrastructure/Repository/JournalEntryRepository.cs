using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MomentOfUs.Domain.Contracts;
using MomentOfUs.Domain.Models;

namespace MomentOfUs.Infrastructure.Repository
{
    public class JournalEntryRepository : RepositoryBase<JournalEntry>, IJournalEntryRepository
    {
        public JournalEntryRepository(RepositoryContext repositoryContext) : base(repositoryContext)
        {
        }


        /// <summary>
        /// Returns a entry for specified journal id
        /// </summary>
        /// <param name="entryId"></param>
        /// <param name="trackChanges"></param>
        /// <returns></returns>
        async Task<JournalEntry?> IJournalEntryRepository.GetByIdAsync(Guid entryId, bool trackChanges)
        {
            return await base.FindByCondition(j => j.Id == entryId, trackChanges)
            .Include(j => j.Journal)
            .SingleOrDefaultAsync();
        }




        /// <summary>
        /// Returns all the entries for the specified journal.
        /// </summary>
        /// <param name="journalId"></param>
        /// <param name="trackChanges"></param>
        /// <returns></returns>
        async Task<IEnumerable<JournalEntry>> IJournalEntryRepository.GetByJournalIdAsync(Guid journalId, bool trackChanges)
        {
            return await base.FindByCondition(je => je.JournalId == journalId, trackChanges)
            .ToListAsync();
        }

        async Task IJournalEntryRepository.CreateAsync(JournalEntry entry)
        {
            await base.CreateAsync(entry);

        }

        Task IJournalEntryRepository.Update(JournalEntry entry)
        {
            base.Update(entry);
            return Task.CompletedTask;

        }

        Task IJournalEntryRepository.Delete(JournalEntry entry)
        {
            base.Delete(entry);
            return Task.CompletedTask;
        }
    }
}