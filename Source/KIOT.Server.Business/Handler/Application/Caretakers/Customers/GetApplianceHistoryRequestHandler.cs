using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;

using KIOT.Server.Business.Abstractions.Services;
using KIOT.Server.Business.Request.Application.Caretaker.Customers;
using KIOT.Server.Business.Response.Application.Caretakers.Customers;
using KIOT.Server.Core.Data.Api.Request;
using KIOT.Server.Core.Data.Persistence;
using KIOT.Server.Core.Data.Persistence.Application;
using KIOT.Server.Core.Models.Application;
using KIOT.Server.Core.Response;
using KIOT.Server.Dto.Application.Customers.Appliances;
using KIOT.Server.Dto.Application.Customers.Data;
using KIOT.Server.Dto.Data;

namespace KIOT.Server.Business.Handler.Application.Caretakers.Customers
{
    internal class GetApplianceHistoryRequestHandler : IRequestHandler<GetApplianceHistoryRequest, GetApplianceHistoryResponse>
    {
        private readonly IApiClient _client;
        private readonly IValidator<GetApplianceHistoryRequest> _validator;
        private readonly IApplianceService _applianceService;
        private readonly IUnitOfWork<ICaretakerRepository, Caretaker> _unitOfWork;

        private static readonly IDictionary<TimeInterval, int> SkipsDictionary = new Dictionary<TimeInterval, int>
        {
            { TimeInterval.Day, 240 },
            { TimeInterval.Week, 48 },
            { TimeInterval.Month, 168 },
            { TimeInterval.Year, 30 },
        };

        public GetApplianceHistoryRequestHandler(IApiClient client,
            IValidator<GetApplianceHistoryRequest> validator, IApplianceService applianceService,
            IUnitOfWork<ICaretakerRepository, Caretaker> unitOfWork)
        {
            _client = client;
            _validator = validator;
            _applianceService = applianceService;
            _unitOfWork = unitOfWork;
        }

        public async Task<GetApplianceHistoryResponse> Handle(GetApplianceHistoryRequest request,
            CancellationToken cancellationToken)
        {
            if (!_validator.Validate(request).IsValid)
            {
                return new GetApplianceHistoryResponse(new []{ new Error("InvalidRequest", "Parameters are incorrect.") });
            }

            var caretaker = await _unitOfWork.Repository.GetByGuidAsync(request.CaretakerGuid,
                $"{nameof(Caretaker.TakingCareOf)}.{nameof(CaretakerForCustomer.Customer)}");

            if (!caretaker.IsTakingCareOf(request.CustomerGuid))
            {
                return new GetApplianceHistoryResponse(new []{ new Error("NoSuchCustomer", "No such Customer is registered with Caretaker") });
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
                return new GetApplianceHistoryResponse(new []{ new Error("CouldNotFetchData", "Remote Api did not return any data") });
            }

            var skips = SkipsDictionary[request.Interval];

            var startTime = DateTimeOffset.FromUnixTimeSeconds(response.Timestamps.FirstOrDefault());
            var intervalStart = response.Timestamps.Count - skips * (request.IntervalOffset + 1);

            if (intervalStart < 0)
            {
                return new GetApplianceHistoryResponse(new []
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
                    Appliance = applianceMap.ContainsKey((int)c.ApplianceId)
                        ? applianceMap[(int)c.ApplianceId]
                        : new ApplianceDto { ApplianceId = (int) c.ApplianceId, ApplianceTypeId = p.ApplianceTypeId },
                    TotalAverageUsage = (short?) c.CalculateTotalAverage(),
                    CurrentAverageUsage = (short?) c.PowerConsumption.Skip(intervalStart)
                        .Take(intervalStop - intervalStart).Average(x => x ?? 0),
                    UsagePoints = c.GetIntervalEveryNthPoint(dataPointFreq)
                };
            }).ToList();

            _applianceService.AddApplianceInfo(appliances.Select(x => x.Appliance), applianceInfo);

            return new GetApplianceHistoryResponse(new CustomerDetailedPageDto
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
