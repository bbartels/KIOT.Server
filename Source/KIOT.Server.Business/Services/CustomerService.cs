using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using KIOT.Server.Business.Abstractions.Services;
using KIOT.Server.Core.Data.Api.Request;
using KIOT.Server.Core.Data.Persistence;
using KIOT.Server.Core.Data.Persistence.Application;
using KIOT.Server.Core.Services;

using ApplianceType = KIOT.Server.Core.Models.Application.ApplianceType;
using Customer = KIOT.Server.Core.Models.Data.Customer;

namespace KIOT.Server.Business.Services
{
    internal class CustomerService : ICustomerService
    {
        private readonly IApiClient _client;
        private readonly IUnitOfWork<IApplianceTypeRepository, ApplianceType> _unitOfWork;
        private readonly IUnitOfWork<ICustomerRepository, Core.Models.Application.Customer> _cUnitOfWork;
        private readonly IApplianceService _applianceService;

        private static DateTime _cachedTimestamp;
        private static ConcurrentDictionary<string, Customer> _customerMap = new ConcurrentDictionary<string, Customer>();

        public CustomerService(IApiClient client, IUnitOfWork<IApplianceTypeRepository, ApplianceType> unitOfWork,
            IUnitOfWork<ICustomerRepository, Core.Models.Application.Customer> cUnitOfWork, IApplianceService applianceService)
        {
            _client = client;
            _unitOfWork = unitOfWork;
            _cUnitOfWork = cUnitOfWork;
            _applianceService = applianceService;
        }

        public async Task<IEnumerable<Customer>> GetCustomers()
        {
            return _cachedTimestamp >= DateTime.UtcNow.AddMinutes(-5)
                ? _customerMap.Values
                : await GetAndCacheCustomers();

            async Task<IEnumerable<Customer>> GetAndCacheCustomers()
            {
                _customerMap = new ConcurrentDictionary<string, Customer>((await _client.GetAllCustomers()).ToDictionary(x => x.CustomerId.Code, x => x));
                _cachedTimestamp = DateTime.UtcNow;
                return _customerMap.Values;
            }
        }

        public async Task UpdateApplianceTypes()
        {
            var oldTypes = (await _unitOfWork.Repository.GetAsync()).ToDictionary(x => x.ApplianceTypeId, x => x);
            var newTypes = _customerMap.Values.SelectMany(x => x.Sensors.Select(y => y.ObservationTarget))
                .GroupBy(x => x.ApplianceTypeId).Select(x => x.FirstOrDefault());

            foreach (var type in newTypes)
            {
                if (type == null) { continue; }
                if (oldTypes.ContainsKey(type.ApplianceTypeId))
                {
                    oldTypes[type.ApplianceTypeId].SetName(type.Name);
                }

                else
                {
                    _unitOfWork.Repository.Add(new ApplianceType(type.ApplianceTypeId, type.Name));
                }
            }

            try
            {
                await _unitOfWork.SaveChangesAsync();
            }
            catch (Exception) { }
        }

        public async Task RunTasks()
        {
            _ = await GetCustomers();
            await UpdateApplianceTypes();
        }
    }
}
