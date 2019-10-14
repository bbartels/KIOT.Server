using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Hangfire;

using KIOT.Server.Core.Services;

namespace KIOT.Server.Api.Services
{
    public class BackgroundTaskService : IBackgroundTaskService
    {
        public void EnqueueTask(Expression<Func<Task>> task)
        {
            BackgroundJob.Enqueue(task);
        }
    }
}
