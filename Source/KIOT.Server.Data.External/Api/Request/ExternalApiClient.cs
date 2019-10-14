using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;

using KIOT.Server.Core.Data.Api;
using KIOT.Server.Core.Data.Api.Request;
using KIOT.Server.Core.Models.Data;
using KIOT.Server.Core.Request.Api;
using KIOT.Server.Dto.Application.Appliances;
using KIOT.Server.Dto.Application.Customers.Appliances;
using KIOT.Server.Dto.Application.Customers.Data;
using ApplianceType = KIOT.Server.Core.Models.Data.ApplianceType;
using Customer = KIOT.Server.Core.Models.Data.Customer;

namespace KIOT.Server.Data.External.Api.Request
{
    internal class ExternalApiClient : IApiClient
    {
        private readonly HttpClient _client;
        private readonly IRequestBuilder _requestBuilder;
        private readonly IJsonEntityParser _jsonParser;

        public ExternalApiClient(IApiHttpClientAccessor accessor, IRequestBuilder requestBuilder, IJsonEntityParser parser)
        {
            _client = accessor.Client;
            _requestBuilder = requestBuilder;
            _jsonParser = parser;
        }

        public async Task<string> GetResponseAsync(IApiRequest request)
        {
            return await (await MakeRequestAsync(request)).Content.ReadAsStringAsync();
        }

        public async Task<IEnumerable<string>> GetResponseAsync(IEnumerable<IApiRequest> requests)
        {
            return await Task.WhenAll((await MakeRequestAsync(requests))
                             .Select(x => x.Content.ReadAsStringAsync()));
        }

        public async Task<IEnumerable<Customer>> GetResponseAsync(IGetCustomerRequest request)
        {
            var json = await (await MakeRequestAsync(request)).Content.ReadAsStringAsync();
            return _jsonParser.ConvertCustomer(json).Customers;
        }

        public async Task<ObservedData> GetResponseAsync(IGetObservedDataRequest request)
        {
            var json = await (await MakeRequestAsync(request)).Content.ReadAsStringAsync();
            return _jsonParser.ConvertObservedData(json).Data;
        }

        public async Task<bool> CustomerExists(CustomerCode code)
        {
            var request = new GetObservedDataRequest(code, new SensorTimestamp(), new SensorTimestamp());
            return (await MakeRequestAsync(request)).IsSuccessStatusCode;
        }

        public async Task<ObservedDataResponse> GetCustomerAppliances(CustomerCode code, TimeInterval ti, ushort intervalOffset)
        {
            if (!Enum.IsDefined(typeof(TimeInterval), ti)) { throw new ArgumentException($"{nameof(TimeInterval)}: {ti} is not defined."); }

            Dictionary<TimeInterval, (int dayOffset , TimeUnit timeUnit)> test = new Dictionary<TimeInterval, (int, TimeUnit)>
            {
                { TimeInterval.Year,  (-(int)TimeInterval.Year,  TimeUnit.Day) },
                { TimeInterval.Month, (-(int)TimeInterval.Month, TimeUnit.Hour) },
                { TimeInterval.Week,  (-(int)TimeInterval.Week,  TimeUnit.HalfHour) },
                { TimeInterval.Day,   (-(int)TimeInterval.Day,   TimeUnit.Minute) },
            };

            var request = new GetObservedDataRequest(code, new SensorTimestamp(test[ti].dayOffset),
                new SensorTimestamp(DateTimeOffset.UtcNow), new TimeUnitId(test[ti].timeUnit));
            return _jsonParser.ConvertObservedData(await (await MakeRequestAsync(request)).Content.ReadAsStringAsync());
        }

        public async Task<IEnumerable<Customer>> GetAllCustomers()
        {
            var request = new GetCustomerRequest();
            return _jsonParser.ConvertCustomer(await (await MakeRequestAsync(request)).Content.ReadAsStringAsync()).Customers;
        }

        public async Task<IEnumerable<ApplianceDto>> GetCustomerAppliances(CustomerCode code)
        {
            var offset = DateTimeOffset.UtcNow.Subtract(TimeSpan.FromDays(100));
            var request = new GetObservedDataRequest(code, new SensorTimestamp(offset));
            var detailedRequest = new GetObservedDataRequest(code, new SensorTimestamp(DateTimeOffset.UtcNow.Subtract(TimeSpan.FromHours(3))), null, new TimeUnitId(TimeUnit.Minute));
            var response = (await MakeRequestAsync(new []{request, detailedRequest})).ToList();
            var appliances = _jsonParser.ConvertObservedData(await response[0].Content.ReadAsStringAsync());
            var detailedMap = _jsonParser.ConvertObservedData(await response[1].Content.ReadAsStringAsync())
                .Data.ApplianceTypes.SelectMany(x => x.Appliances).ToDictionary(x => x.ApplianceId, x => x);

            DateTime GetLastUsed(uint applianceId, Appliance appliance)
            {
                if (!detailedMap.ContainsKey(applianceId)) { return DateTime.MinValue; }

                var detailedUsed = detailedMap[applianceId].ApplianceLastUsed();
                return detailedUsed ?? appliance.ApplianceLastUsed() ?? offset.DateTime;
            }

            return appliances.Data.ApplianceTypes.SelectMany(x => x.Appliances, (type, app) => 
                new ApplianceDto
                {
                    ApplianceId = (int)app.ApplianceId,
                    ApplianceTypeId = type.ApplianceTypeId,
                    LastActivity = GetLastUsed(app.ApplianceId, app)
                });
        }

        public async Task<IEnumerable<ApplianceActivityDto>> GetApplianceActivity(CustomerCode code)
        {
            var offset = DateTimeOffset.UtcNow.Subtract(TimeSpan.FromDays(365));
            var dailyHistoryRequest = new GetObservedDataRequest(code, new SensorTimestamp(offset), null, new TimeUnitId(TimeUnit.Day));
            var hourlyHistoryRequest = new GetObservedDataRequest(code, new SensorTimestamp(DateTimeOffset.UtcNow.Subtract(TimeSpan.FromHours(23))), null, new TimeUnitId(TimeUnit.Minute));

            var response = (await MakeRequestAsync(new[] {  hourlyHistoryRequest, dailyHistoryRequest })).ToList();
            var hourlyHistoryMap = _jsonParser.ConvertObservedData(await response[0].Content.ReadAsStringAsync())
                .Data.ApplianceTypes.SelectMany(x => x.Appliances).ToDictionary(x => x.ApplianceId, x => x);
            var dailyHistoryMap = _jsonParser.ConvertObservedData(await response[1].Content.ReadAsStringAsync())
                .Data.ApplianceTypes.SelectMany(x => x.Appliances).ToDictionary(x => x.ApplianceId, x => x);

            return hourlyHistoryMap.Keys.Select(x => new ApplianceActivityDto
            {
                ApplianceId = (int) x,
                LastActivity = hourlyHistoryMap[x].ApplianceLastUsed() ??
                               dailyHistoryMap[x].ApplianceLastUsed() ?? DateTime.MinValue
            });
        }

        public async Task<(Appliance appliance, ApplianceType type)?> GetCustomerAppliance(CustomerCode code, int applianceId)
        {
            var request = new GetObservedDataRequest(code, new SensorTimestamp(DateTimeOffset.UtcNow.Subtract(TimeSpan.FromDays(30))));
            var response = _jsonParser.ConvertObservedData(await (await MakeRequestAsync(request)).Content.ReadAsStringAsync());

            var appliance = response.Data.ApplianceTypes
                .SelectMany(x => x.Appliances, (type, app) => new { Appliance = app, Type = type })
                .SingleOrDefault(x => x.Appliance.ApplianceId == applianceId);

            return appliance != null ? (appliance.Appliance, appliance.Type) : ((Appliance, ApplianceType)?) null;
        }

        private async Task<HttpResponseMessage> MakeRequestAsync(IApiRequest request)
        {
            return (await MakeRequestAsync(new[] { request })).First();
        }

        private async Task<IEnumerable<HttpResponseMessage>> MakeRequestAsync(IEnumerable<IApiRequest> requests)
        {
            var tasks = requests.Select(request => _client.GetAsync(_requestBuilder.BuildRequest(request))).ToList();
            return await Task.WhenAll(tasks);
        }
    }
}
