using System;
using System.Threading.Tasks;

using KIOT.Server.Business.Abstractions.Jobs;
using KIOT.Server.Business.Abstractions.Services;
using KIOT.Server.Core.Data.Persistence;
using KIOT.Server.Core.Data.Persistence.Application;
using KIOT.Server.Core.Models.Application;

namespace KIOT.Server.Business.Jobs
{
    internal class CacheCustomerApplianceHistoryRequest : IJobRequest
    {
        public Guid CustomerGuid { get; }

        public CacheCustomerApplianceHistoryRequest(Guid customerGuid)
        {
            CustomerGuid = customerGuid;
        }
    }

    internal class CacheCustomerApplianceHistoryJob : IBackgroundJob<CacheCustomerApplianceHistoryRequest>
    {
        private readonly IUnitOfWork<ICaretakerRepository, Caretaker> _unitOfWork;
        private readonly IApplianceService _applianceService;
        public CacheCustomerApplianceHistoryJob(IUnitOfWork<ICaretakerRepository, Caretaker> unitOfWork,
            IApplianceService applianceService)
        {
            _unitOfWork = unitOfWork;
            _applianceService = applianceService;
        }

        public async Task Run(CacheCustomerApplianceHistoryRequest request)
        {
            await _applianceService.CacheCustomerApplianceHistory(request.CustomerGuid);
        }
    }
}
