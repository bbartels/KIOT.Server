using System;

using KIOT.Server.Business.Jobs;

namespace KIOT.Server.Business.Abstractions.Services
{
    internal interface IBackgroundTaskScheduler
    {
        void ScheduleTask(Guid customerGuid);
        void ScheduleTask(CacheCustomersForCaretakerRequest request);
        void ScheduleTask(CacheCustomerApplianceHistoryRequest request);
    }
}
