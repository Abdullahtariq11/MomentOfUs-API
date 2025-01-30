using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MomentOfUs.Domain.Contracts
{
    public interface IRepositoryManager
    {
        IJournalRepository JournalRepository { get; }
        ISharedJournalRepository SharedJournalRepository{ get; }
        IUserSharedJournalRepository UserSharedJournalRepository { get; }

        Task SaveAsync();
    }
}