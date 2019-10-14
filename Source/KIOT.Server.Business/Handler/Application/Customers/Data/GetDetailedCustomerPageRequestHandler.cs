using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;

using KIOT.Server.Business.Abstractions.Services;
using KIOT.Server.Business.Request.Application.Customer.Data;
using KIOT.Server.Business.Response.Application.Customers;
using KIOT.Server.Core.Data.Api.Request;
using KIOT.Server.Core.Data.Persistence;
using KIOT.Server.Core.Data.Persistence.Application;
using KIOT.Server.Core.Response;
using KIOT.Server.Dto.Application.Customers.Data;
using KIOT.Server.Dto.Data;
using Customer = KIOT.Server.Core.Models.Application.Customer;

namespace KIOT.Server.Business.Handler.Application.Customers.Data
{
    internal class GetDetailedCustomerPageRequestHandler : IRequestHandler<GetDetailedCustomerPageRequest, GetDetailedCustomerPageResponse>
    {
        private readonly IApiClient _client;
        private readonly IValidator<GetDetailedCustomerPageRequest> _validator;
        private readonly IApplianceService _applianceService;
        private readonly IUnitOfWork<ICustomerRepository, Customer> _unitOfWork;

        private static readonly IDictionary<TimeInterval, int> SkipsDictionary = new Dictionary<TimeInterval, int>
        {
            { TimeInterval.Day, 240 },
            { TimeInterval.Week, 48 },
            { TimeInterval.Month, 168 },
            { TimeInterval.Year, 30 },
        };

        public GetDetailedCustomerPageRequestHandler(IApiClient client,
            IValidator<GetDetailedCustomerPageRequest> validator, IApplianceService applianceService,
            IUnitOfWork<ICustomerRepository, Customer> unitOfWork)
        {
            _client = client;
            _validator = validator;
            _applianceService = applianceService;
            _unitOfWork = unitOfWork;
        }

        public async Task<GetDetailedCustomerPageResponse> Handle(GetDetailedCustomerPageRequest request,
            CancellationToken cancellationToken)
        {
            if (!_validator.Validate(request).IsValid)
            {
                return new GetDetailedCustomerPageResponse(new []{ new Error("InvalidRequest", "Parameters are incorrect.") });
            }

            var applianceInfoTask = _applianceService.GetApplianceInfo(request.CustomerGuid);
            var applianceTask = _applianceService.GetCustomerAppliances(request.CustomerGuid);

            var responseTask = _applianceService.GetCustomerApplianceHistory(request.CustomerGuid, request.Interval, request.IntervalOffset);
            await Task.WhenAll(responseTask, applianceInfoTask, applianceTask);

            var applianceMap = applianceTask.Result.ToDictionary(x => x.ApplianceId, x => x);
            var applianceInfo = applianceInfoTask.Result;
            var response = responseTask.Result;

            if (response == null)
            {
                return new GetDetailedCustomerPageResponse(new []{ new Error("CouldNotFetchData", "Remote Api did not return any data") });
            }

            var skips = SkipsDictionary[request.Interval];

            var startTime = DateTimeOffset.FromUnixTimeSeconds(response.Timestamps.FirstOrDefault());
            var intervalStart = response.Timestamps.Count - skips * (request.IntervalOffset + 1);

            if (intervalStart < 0)
            {
                return new GetDetailedCustomerPageResponse(new []
                {
                    new Error("InvalidRequest", $"Invalid {nameof(request.IntervalOffset)} parameter: {request.IntervalOffset}.")
                });
            }

            var intervalStop = intervalStart + skips - 1;

            var intervalStartTime = DateTimeOffset.FromUnixTimeSeconds(response.Timestamps[intervalStart]);
            var intervalStopTime = DateTimeOffset.FromUnixTimeSeconds(response.Timestamps[intervalStop]);
            var endTime = DateTimeOffset.FromUnixTimeSeconds(response.Timestamps.LastOrDefault());
            var dataPointFreq = response.Timestamps.Count / 60;

            var appliances = response.ApplianceTypes.SelectMany(x => x.Appliances, (p, c) =>
            {
                return new ApplianceUsageDto
                {
                    Appliance = applianceMap.ContainsKey((int)c.ApplianceId) ? applianceMap[(int)c.ApplianceId] : null,
                    TotalAverageUsage = (short?) c.CalculateTotalAverage(),
                    CurrentAverageUsage = (short?) c.PowerConsumption.Skip(intervalStart)
                        .Take(intervalStop - intervalStart).Average(x => x ?? 0),
                    UsagePoints = c.GetIntervalEveryNthPoint(dataPointFreq)
                };
            }).ToList();

            _applianceService.AddApplianceInfo(appliances.Select(x => x.Appliance), applianceInfo);

            return new GetDetailedCustomerPageResponse(new CustomerDetailedPageDto
            {
                StartTime = startTime,
                StartTimeInterval = intervalStartTime,
                EndTimeInterval = intervalStopTime,
                EndTime = endTime,
                Appliances = appliances,
                DataPointEvery = response.Timestamps[dataPointFreq] - response.Timestamps[0]
            });
        }
    }
}
