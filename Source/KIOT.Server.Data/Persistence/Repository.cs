using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

using KIOT.Server.Core.Data.Persistence;
using KIOT.Server.Core.Models;

namespace KIOT.Server.Data.Persistence
{
    internal class Repository<T> : IRepository<T> where T : BaseEntity
    {
        protected DbContext Context { get; }

        public Repository(DbContext context)
        {
            Context = context;
        }

        public async Task<T> GetByIdAsync(int id, string includes = null)
        {
            return await BuildQueryable(x => x.Id == id, null, includes).SingleOrDefaultAsync();
        }

        public Task<T> GetByGuidAsync(Guid guid, string includes = null)
        {
            return BuildQueryable(x => x.Guid == guid, null, includes).SingleOrDefaultAsync();
        }

        public async Task<int?> GetIdAsync(Expression<Func<T, bool>> predicate)
        {
            return await BuildQueryable(predicate).Select(x => x.Id).SingleOrDefaultAsync();
        }

        public async Task<IEnumerable<T>> GetAsync(Expression<Func<T, bool>> predicate = null, 
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, 
            string includes = null, 
            int? skip = null,
            int? take = null)
        {
            return await BuildQueryable(predicate, orderBy, includes, skip, take).ToListAsync();
        }

        public async Task<T> SingleOrDefaultAsync(Expression<Func<T, bool>> predicate, string includes = null)
        {
            return await BuildQueryable(predicate, null, includes).SingleOrDefaultAsync();
        }

        public async Task<int> GetCountAsync(Expression<Func<T, bool>> predicate = null)
        {
            return await BuildQueryable(predicate).CountAsync();
        }

        public async Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate = null, string includes = null)
        {
            return await BuildQueryable(predicate, null, includes).AnyAsync();
        }

        public void Add(T entity)
        {
            Context.Set<T>().Add(entity);
        }

        public void AddRange(IEnumerable<T> entities)
        {
            Context.Set<T>().AddRange(entities);
        }


        public void Update(T entity)
        {
            Context.Set<T>().Attach(entity);
            Context.Entry(entity).State = EntityState.Modified;
        }

        public void Remove(T entity)
        {
            Context.Set<T>().Remove(entity);
        }

        public void RemoveRange(IEnumerable<T> entities)
        {
            Context.Set<T>().RemoveRange(entities);
        }
        protected virtual IQueryable<T> BuildQueryable(
            Expression<Func<T, bool>> predicate = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            string includes = null,
            int? skip = null,
            int? take = null)
        {
            includes = includes ?? string.Empty;
            IQueryable<T> query = Context.Set<T>();

            if (predicate != null) { query = query.Where(predicate); }

            query = includes.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                .Aggregate(query, (current, include) => current.Include(include));

            if (orderBy != null) { query = orderBy(query); }

            if (skip.HasValue) { query = query.Skip(skip.Value); }
            if (take.HasValue) { query = query.Take(take.Value); }

            return query;
        }
    }
}
