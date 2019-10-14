using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

using KIOT.Server.Core.Models;

namespace KIOT.Server.Core.Data.Persistence
{
    /// <summary>
    /// Interface for generic Repository.
    /// </summary>
    /// <typeparam name="T">Domain type of Repository.</typeparam>
    public interface IRepository<T> where T : BaseEntity
    {
        Task<T> GetByIdAsync(int id, string includes = null);

        Task<T> GetByGuidAsync(Guid guid, string includes = null);

        Task<int?> GetIdAsync(Expression<Func<T, bool>> predicate);

        Task<IEnumerable<T>> GetAsync(Expression<Func<T, bool>> predicate = null, 
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, 
            string includes = null,
            int? skip = null,
            int? take = null);

        Task<T> SingleOrDefaultAsync(Expression<Func<T, bool>> predicate, string includes = null);
       
        Task<int> GetCountAsync(Expression<Func<T, bool>> predicate = null);
        
        Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate = null, string includes = null);

        void Add(T entity);

        void AddRange(IEnumerable<T> entities);
        void Update(T entity);
     
        void Remove(T entity);
     
        void RemoveRange(IEnumerable<T> entities);
    }
}
