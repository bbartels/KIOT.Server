using System;
using Microsoft.Extensions.DependencyInjection;

using KIOT.Server.Business.Abstractions.Jobs;
using KIOT.Server.Business.Abstractions.Services;
using KIOT.Server.Business.Jobs;
using KIOT.Server.Core.Services;

namespace KIOT.Server.Business.Services
{
    internal class BackgroundTaskScheduler : IBackgroundTaskScheduler
    {
        private readonly IApplianceService _applianceService;
        private readonly IBackgroundTaskService _taskService;
        private readonly IServiceProvider _serviceProvider;

        public BackgroundTaskScheduler(IApplianceService applianceService, IBackgroundTaskService taskService, IServiceProvider serviceProvider)
        {
            _applianceService = applianceService;
            _taskService = taskService;
            _serviceProvider = serviceProvider;
        }

        public void ScheduleTask(Guid customerGuid)
        {
            _taskService.EnqueueTask(() => _applianceService.CacheCustomerApplianceHistory(customerGuid));
        }

        public void ScheduleTask(CacheCustomersForCaretakerRequest request)
        {
            _taskService.EnqueueTask(() =>
                _serviceProvider.GetService<IBackgroundJob<CacheCustomersForCaretakerRequest>>().Run(request));
        }

        public void ScheduleTask(CacheCustomerApplianceHistoryRequest request)
        {
            _taskService.EnqueueTask(() =>
                _serviceProvider.GetService<IBackgroundJob<CacheCustomerApplianceHistoryRequest>>().Run(request));
        }
    }
}
