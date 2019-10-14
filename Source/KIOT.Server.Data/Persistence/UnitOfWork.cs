using System;
using System.Threading.Tasks;

using KIOT.Server.Core.Data.Persistence;
using KIOT.Server.Core.Models;

namespace KIOT.Server.Data.Persistence
{
    internal class UnitOfWork<TRepository, TEntity> : IUnitOfWork<TRepository, TEntity> 
                                            where TEntity : BaseEntity
                                            where TRepository : IRepository<TEntity>
    {
        private readonly KIOTContext _context;

        public TRepository Repository { get; }

        public UnitOfWork(KIOTContext context, TRepository repository)
        {
            _context = context;
            Repository = repository;
        }

        public void SaveChanges()
        {
            _context.SaveChanges();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        private bool _disposed;

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed && disposing) { _context.Dispose(); }
            _disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
