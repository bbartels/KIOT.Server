using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace KIOT.Server.Core.Services
{
    public interface IBackgroundTaskService
    {
        void EnqueueTask(Expression<Func<Task>> task);
    }
}
