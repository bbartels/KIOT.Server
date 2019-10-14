using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using KIOT.Server.Business.Abstractions.Services;
using KIOT.Server.Core.Data.Api.Request;
using KIOT.Server.Core.Data.Persistence;
using KIOT.Server.Core.Data.Persistence.Application;
using KIOT.Server.Core.Models.Application;
using KIOT.Server.Core.Models.Data;
using KIOT.Server.Dto.Application.Appliances;
using KIOT.Server.Dto.Application.Customers.Appliances;
using KIOT.Server.Dto.Application.Customers.Data;
using Customer = KIOT.Server.Core.Models.Application.Customer;

namespace KIOT.Server.Business.Services
{
    internal class ApplianceService : IApplianceService
    {
        private readonly IApiClient _client;
        private readonly IUnitOfWork<ICustomerApplianceRepository, CustomerAppliance> _unitOfWork;
        private readonly IUnitOfWork<ICustomerRepository, Customer> _cUnitOfWork;
        private readonly IApplianceServiceConfiguration _configuration;

        private static readonly ConcurrentDictionary<Guid, (DateTime entryDate, IEnumerable<ApplianceDto> appliances)> ApplianceMap =
            new ConcurrentDictionary<Guid, (DateTime entryDate, IEnumerable<ApplianceDto> appliances)>();

        private static readonly ConcurrentDictionary<Guid, CustomerCode> CustomerCodeMap = new ConcurrentDictionary<Guid, CustomerCode>();
        private static readonly ConcurrentDictionary<(Guid, TimeInterval, int), (DateTime entryDate, ObservedData data)> ApplianceHistoryMap =
            new ConcurrentDictionary<(Guid, TimeInterval, int), (DateTime entryDate, ObservedData data)>();
        private static readonly ConcurrentDictionary<Guid, (DateTime entryTime, IEnumerable<ApplianceActivityDto>)> ApplianceLastActiveMap =
            new ConcurrentDictionary<Guid, (DateTime entryTime, IEnumerable<ApplianceActivityDto>)>();

        public ApplianceService(IApiClient client, IUnitOfWork<ICustomerApplianceRepository, CustomerAppliance> unitOfWork,
            IUnitOfWork<ICustomerRepository, Customer> cUnitOfWork, IApplianceServiceConfiguration configuration)
        {
            _client = client;
            _unitOfWork = unitOfWork;
            _cUnitOfWork = cUnitOfWork;
            _configuration = configuration;
        }

        public async Task<IDictionary<int, CustomerAppliance>> GetApplianceInfo(Guid customerGuid)
        {
            return (await _unitOfWork.Repository.GetAsync(x => x.Customer.Guid == customerGuid, null,
                $"{nameof(CustomerAppliance.Customer)},{nameof(CustomerAppliance.Category)},{nameof(CustomerAppliance.ApplianceType)}"))
                .ToDictionary(x => x.ApplianceId, x => x);
        }

        public void AddApplianceInfo(IEnumerable<ApplianceDto> appliances, IDictionary<int, CustomerAppliance> applianceInfo)
        {
            foreach (var appliance in appliances)
            {
                if (!applianceInfo.ContainsKey(appliance.ApplianceId)) { continue; }

                var info = applianceInfo[appliance.ApplianceId];
                appliance.Category = info.Category == null ? null : new ApplianceCategoryDto
                {
                    Guid = info.Category.Guid,
                    CategoryName = info.Category.Name,
                };
                appliance.ApplianceName = info.Alias ?? info.Name;
            }
        }

        public async Task<IEnumerable<ApplianceDto>> GetCustomerAppliances(Guid customerGuid)
        {
            var code = await GetAndCacheCustomerCode(customerGuid);

            if (!ApplianceMap.ContainsKey(customerGuid)) { return await GetAndCacheAppliances(); }

            var (entryDate, appliances) = ApplianceMap[customerGuid];

            return entryDate >= DateTime.UtcNow.AddSeconds(-_configuration.ApplianceCacheTimeout) 
                ? appliances
                : await GetAndCacheAppliances();

            async Task<IEnumerable<ApplianceDto>> GetAndCacheAppliances()
            {
                var response = (await _client.GetCustomerAppliances(code)).ToList();
                ApplianceMap[customerGuid] = (DateTime.UtcNow, response);
                return response;
            }
        }

        public async Task<ApplianceDto> GetCustomerAppliance(Guid customerGuid, int applianceId)
        {
            var appliances = await GetCustomerAppliances(customerGuid);
            return appliances.SingleOrDefault(x => x.ApplianceId == applianceId);
        }

        public async Task<IEnumerable<ApplianceActivityDto>> GetCustomerAppliancesActivity(Guid customerGuid)
        {
            if (!ApplianceLastActiveMap.ContainsKey(customerGuid)) { return await GetAndCacheLastActivity(); }

            var (entryTime, activity) = ApplianceLastActiveMap[customerGuid];

            return entryTime >= DateTime.UtcNow.AddSeconds(-_configuration.ActivityCacheTimeout)
                ? activity
                : await GetAndCacheLastActivity();

            async Task<IEnumerable<ApplianceActivityDto>> GetAndCacheLastActivity()
            {
                var code = await GetAndCacheCustomerCode(customerGuid);
                var response = (await _client.GetApplianceActivity(code)).ToList();
                ApplianceLastActiveMap[customerGuid] = (DateTime.UtcNow, response);
                return response;
            }
        }

        public async Task<ObservedData> GetCustomerApplianceHistory(Guid customerGuid, TimeInterval time,
            ushort intervalOffset)
        {
            if (!ApplianceHistoryMap.ContainsKey((customerGuid, time, intervalOffset))) { return await GetAndCacheHistory(); }

            var (entryTime, history) = ApplianceHistoryMap[(customerGuid, time, intervalOffset)];

            return entryTime >= DateTime.UtcNow.AddSeconds(-_configuration.ApplianceCacheTimeout)
                ? history
                : await GetAndCacheHistory();

            async Task<ObservedData> GetAndCacheHistory()
            {
                var code = await GetAndCacheCustomerCode(customerGuid);
                var response = (await _client.GetCustomerAppliances(code, time, intervalOffset)).Data;
                ApplianceHistoryMap[(customerGuid, time, intervalOffset)] = (DateTime.UtcNow, response);
                return response;
            }
        }

        public async Task CacheCustomerApplianceHistory(Guid customerGuid)
        {
            if (!_configuration.BackgroundJobsEnabled) { return; }
            var dailyTask = GetCustomerApplianceHistory(customerGuid, TimeInterval.Day, 0);
            var weekTask = GetCustomerApplianceHistory(customerGuid, TimeInterval.Week, 0);
            var monthTask = GetCustomerApplianceHistory(customerGuid, TimeInterval.Month, 0);
            var yearTask = GetCustomerApplianceHistory(customerGuid, TimeInterval.Year, 0);
            await Task.WhenAll(dailyTask, weekTask, monthTask, yearTask);
        }

        private async Task<CustomerCode> GetAndCacheCustomerCode(Guid customerGuid)
        {
            var code = CustomerCodeMap.ContainsKey(customerGuid)
                ? CustomerCodeMap[customerGuid]
                : await _cUnitOfWork.Repository.GetCustomerCode(customerGuid);

            CustomerCodeMap[customerGuid] = code;
            return code;
        }
    }
}
