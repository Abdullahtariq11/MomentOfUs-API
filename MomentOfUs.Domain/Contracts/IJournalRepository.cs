using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MomentOfUs.Domain.Models;

namespace MomentOfUs.Domain.Contracts
{
    public interface IJournalRepository : IRepositoryBase<Journal>
    {
        void Create(Journal journal);
        void Delete(Journal journal);
        void Update(Journal journal);
        Task<Journal?> GetByIdAsync(Guid journalId, bool trackChanges);
        Task<IEnumerable<Journal>> GetUserJournalsAsync(string userId, bool trackChanges);
        Task<bool> IsOwnerAsync(Guid journalId, string userId);

        
    }
}