using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation;
using MediatR;

using KIOT.Server.Business.Abstractions.Services;
using KIOT.Server.Business.Request.Application.Customer.Data;
using KIOT.Server.Business.Response.Application.Customers;
using KIOT.Server.Core.Data.Api.Request;
using KIOT.Server.Core.Data.Persistence;
using KIOT.Server.Core.Data.Persistence.Application;
using KIOT.Server.Core.Response;
using KIOT.Server.Dto.Application.Customers;
using KIOT.Server.Dto.Application.Customers.Data;
using KIOT.Server.Dto.Data;
using Customer = KIOT.Server.Core.Models.Application.Customer;

namespace KIOT.Server.Business.Handler.Application.Customers.Data
{
    internal class GetCustomerHomepageRequestHandler : IRequestHandler<GetCustomerHomepageRequest, CustomerHomepageResponse>
    {
        private const short DataRequestDays = -365;
        private static readonly (short interval, short days)[] AnalyticsData = { (28, 364), (14, 30), (7, 7) };

        private readonly IValidator<GetCustomerHomepageRequest> _validator;
        private readonly IUnitOfWork<ICustomerRepository, Customer> _unitOfWork;
        private readonly IApiClient _client;
        private readonly IMapper _mapper;
        private readonly IApplianceService _applianceService;

        public GetCustomerHomepageRequestHandler(IValidator<GetCustomerHomepageRequest> validator,
            IUnitOfWork<ICustomerRepository, Customer> unitOfWork,
            IApiClient client, IMapper mapper, IApplianceService applianceService)
        {
            _validator = validator;
            _unitOfWork = unitOfWork;
            _client = client;
            _mapper = mapper;
            _applianceService = applianceService;
        }

        public async Task<CustomerHomepageResponse> Handle(GetCustomerHomepageRequest request,
            CancellationToken cancellationToken)
        {
            if (!_validator.Validate(request).IsValid)
            {
                return new CustomerHomepageResponse(new []{ new Error("InvalidRequest",
                    "Could not process request, invalid parameters.") });
            }

            var user = await _unitOfWork.Repository.GetByGuidAsync(request.CustomerGuid);

            var response = await _applianceService.GetCustomerApplianceHistory(request.CustomerGuid, TimeInterval.Year, 0);

            if (response == null)
            {
                return new CustomerHomepageResponse(new[] { new Error("FetchUnsuccessful", "Could not fetch data.") });
            }

            var ratios = new List<PowerUsageOverIntervalDto>();

            for (int i = 0; i < AnalyticsData.Length; i++)
            {
                var (interval, days) = AnalyticsData[i];
                var usage = response.PowerUsageOverMedianOfIntervals(interval, days);
                usage.TimePeriod = (TimePeriod) i;
                ratios.Add(usage);
            }

            var dto = new CustomerHomepageDto
            {
                Customer = _mapper.Map<CustomerInfoDto>(user),
                PowerUsageRatios = ratios
            };

            return new CustomerHomepageResponse(dto);
        }
    }
}
