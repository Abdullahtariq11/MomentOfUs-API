using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MomentOfUs.Domain.Contracts;
using MomentOfUs.Domain.Models;

namespace MomentOfUs.Infrastructure.Repository
{
    public class JournalRepository :RepositoryBase<Journal>, IJournalRepository
    {
        public JournalRepository(RepositoryContext repositoryContext): base(repositoryContext)
        {
            
        }
    }
}