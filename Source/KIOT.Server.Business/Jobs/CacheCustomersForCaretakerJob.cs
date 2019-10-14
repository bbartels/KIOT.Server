using System;
using System.Linq;
using System.Threading.Tasks;

using KIOT.Server.Business.Abstractions.Jobs;
using KIOT.Server.Business.Abstractions.Services;
using KIOT.Server.Core.Data.Persistence;
using KIOT.Server.Core.Data.Persistence.Application;
using KIOT.Server.Core.Models.Application;

namespace KIOT.Server.Business.Jobs
{
    internal class CacheCustomersForCaretakerRequest : IJobRequest
    {
        public Guid CaretakerGuid { get; }

        public CacheCustomersForCaretakerRequest(Guid caretakerGuid)
        {
            CaretakerGuid = caretakerGuid;
        }
    }

    internal class CacheCustomersForCaretakerJob : IBackgroundJob<CacheCustomersForCaretakerRequest>
    {
        private readonly IUnitOfWork<ICaretakerRepository, Caretaker> _unitOfWork;
        private readonly IApplianceService _applianceService;
        public CacheCustomersForCaretakerJob(IUnitOfWork<ICaretakerRepository, Caretaker> unitOfWork,
            IApplianceService applianceService)
        {
            _unitOfWork = unitOfWork;
            _applianceService = applianceService;
        }

        public async Task Run(CacheCustomersForCaretakerRequest request)
        {
            var caretaker = await _unitOfWork.Repository.GetByGuidAsync(request.CaretakerGuid,
                $"{nameof(Caretaker.TakingCareOf)}.{nameof(CaretakerForCustomer.Customer)}");

            var tasks = caretaker.TakingCareOf.Select(x => _applianceService.CacheCustomerApplianceHistory(x.Customer.Guid));
            await Task.WhenAll(tasks);
        }
    }
}
