using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation;
using MediatR;

using KIOT.Server.Business.Abstractions.Services;
using KIOT.Server.Business.Request.Application.Caretaker.Data;
using KIOT.Server.Business.Response.Application.Caretakers;
using KIOT.Server.Core.Data.Persistence;
using KIOT.Server.Core.Data.Persistence.Application;
using KIOT.Server.Core.Models.Application;
using KIOT.Server.Core.Response;
using KIOT.Server.Dto.Application.Caretakers.Data;
using KIOT.Server.Dto.Application.Customers;
using KIOT.Server.Dto.Application.Customers.Data;
using KIOT.Server.Dto.Data;

namespace KIOT.Server.Business.Handler.Application.Caretakers.Customers
{
    internal class GetCustomerHomepageForCaretakerRequestHandler : 
        IRequestHandler<GetCustomerHomepageForCaretakerRequest, CustomerHomepageForCaretakerResponse>
    {
        private const short DataRequestDays = -365;
        private static readonly (short interval, short days)[] AnalyticsData = { (28, 364), (14, 30), (7, 7) };

        private readonly IMapper _mapper;
        private readonly IBackgroundTaskScheduler _scheduler;
        private readonly IApplianceService _applianceService;
        private readonly IValidator<GetCustomerHomepageForCaretakerRequest> _validator;
        private readonly IUnitOfWork<ICaretakerForCustomerRepository, CaretakerForCustomer> _cfcUnitOfWork;

        public GetCustomerHomepageForCaretakerRequestHandler(
            IValidator<GetCustomerHomepageForCaretakerRequest> validator,
            IUnitOfWork<ICaretakerForCustomerRepository, CaretakerForCustomer> cfcUnitOfWork,
            IMapper mapper, IBackgroundTaskScheduler scheduler, IApplianceService applianceService)
        {
            _validator = validator;
            _cfcUnitOfWork = cfcUnitOfWork;
            _mapper = mapper;
            _scheduler = scheduler;
            _applianceService = applianceService;
        }

        public async Task<CustomerHomepageForCaretakerResponse> Handle(GetCustomerHomepageForCaretakerRequest request,
            CancellationToken cancellationToken)
        {
            if (!_validator.Validate(request).IsValid)
            {
                return new CustomerHomepageForCaretakerResponse(new [] { new Error("", "") });
            }

            var cfc = await _cfcUnitOfWork.Repository.SingleOrDefaultAsync(
                x => x.Caretaker.Guid == request.CaretakerGuid && x.Customer.Guid == request.CustomerGuid,
                $"{nameof(Customer)}");

            if (cfc == null)
            {
                return new CustomerHomepageForCaretakerResponse(new []{ new Error("InvalidCustomerGuid",
                    "Could not retrieve Homepage, CustomerGuid is invalid.") });
            }

            var response = await _applianceService.GetCustomerApplianceHistory(request.CustomerGuid, TimeInterval.Year, 0);

            if (response == null)
            {
                return new CustomerHomepageForCaretakerResponse(new[] { new Error("FetchCustomerInfoFailed", 
                    "Could not fetch customer info from remote server.") });
            }

            var ratios = new List<PowerUsageOverIntervalDto>();

            for (int i = 0; i < AnalyticsData.Length; i++)
            {
                var (interval, days) = AnalyticsData[i];
                var usage = response.PowerUsageOverMedianOfIntervals(interval, days);
                usage.TimePeriod = (TimePeriod) i;
                ratios.Add(usage);
            }

            var dto = new CustomerHomepageForCaretakerDto
            {
                Customer = _mapper.Map<CustomerInfoDto>(cfc.Customer),
                PowerUsageRatios = ratios
            };

            _scheduler.ScheduleTask(request.CustomerGuid);

            return new CustomerHomepageForCaretakerResponse(dto);
        }
    }
}
