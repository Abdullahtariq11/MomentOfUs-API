using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace MomentOfUs.Domain.Contracts
{
    /// <summary>
    /// Base CRUD Class All repository classes will inherit this.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IRepositoryBase<T> where T : class
    {
         IQueryable<T> FindAll(bool trackChanges);
         IQueryable<T> FindByCondition(Expression<Func<T, bool>> condition, bool trackChanges);
         void Create(T entity);
         void Update(T entity);
         void Delete(T entity);
    }
}