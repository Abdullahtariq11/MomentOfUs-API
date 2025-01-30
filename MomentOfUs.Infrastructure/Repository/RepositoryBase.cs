using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MomentOfUs.Domain.Contracts;

namespace MomentOfUs.Infrastructure.Repository
{
    /// <summary>
    /// Repository base class, Crud operations
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class RepositoryBase<T> : IRepositoryBase<T> where T : class
    {
        private readonly RepositoryContext _repositoryContext;
        public RepositoryBase(RepositoryContext repositoryContext)
        {
            _repositoryContext = repositoryContext;
        }
        public void Create(T entity)
        {

            _repositoryContext.Set<T>().AddAsync(entity);
        }

        public void Delete(T entity)
        {
            _repositoryContext.Set<T>().Remove(entity);


        }

        public IQueryable<T> FindAll(bool trackChanges)
        {
            return !trackChanges ? _repositoryContext.Set<T>().AsNoTracking() : _repositoryContext.Set<T>().AsTracking();
        }

        public IQueryable<T> FindByCondition(Expression<Func<T, bool>> condition, bool trackChanges)
        {
            return !trackChanges ? _repositoryContext.Set<T>().Where(condition).AsNoTracking() : _repositoryContext.Set<T>().Where(condition);
        }

        public void Update(T entity)
        {
            _repositoryContext.Set<T>().Update(entity);

        }
    }


}