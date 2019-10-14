using System.Threading.Tasks;

using KIOT.Server.Core.Models;

namespace KIOT.Server.Core.Data.Persistence
{
    public interface IUnitOfWork<out TRepository, TEntity> where TEntity : BaseEntity where TRepository : IRepository<TEntity>
    {
        TRepository Repository { get; }

        void SaveChanges();
        Task SaveChangesAsync();
    }
}
