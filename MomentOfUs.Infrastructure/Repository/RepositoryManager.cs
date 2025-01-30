using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MomentOfUs.Domain.Contracts;

namespace MomentOfUs.Infrastructure.Repository
{
    /// <summary>
    /// Manages all the repositories and manages the final change 
    /// </summary>
    public class RepositoryManager : IRepositoryManager
    {
        private readonly RepositoryContext repositoryContext;

        /// <summary>
        /// Use lazy initalization for improvement
        /// </summary>
        private readonly Lazy<JournalRepository> journalRepository;
        private readonly Lazy<SharedJournalRepository> sharedJournalRepository;
        private readonly Lazy<UserSharedJournalRepository> userSharedJournalRepository;

        public RepositoryManager(RepositoryContext _repositoryContext)
        {
            repositoryContext = _repositoryContext;
            journalRepository= new Lazy<JournalRepository>(()=> new JournalRepository(repositoryContext));
            sharedJournalRepository= new Lazy<SharedJournalRepository>(()=> new SharedJournalRepository(repositoryContext));
            userSharedJournalRepository= new Lazy<UserSharedJournalRepository>(()=> new UserSharedJournalRepository(repositoryContext));
            
        }

        public ISharedJournalRepository SharedJournalRepository => sharedJournalRepository.Value;
        public IUserSharedJournalRepository UserSharedJournalRepository => userSharedJournalRepository.Value;

        public IJournalRepository JournalRepository => journalRepository.Value;


        public async Task SaveAsync()
        {
            await repositoryContext.SaveChangesAsync();
        }
    }
}