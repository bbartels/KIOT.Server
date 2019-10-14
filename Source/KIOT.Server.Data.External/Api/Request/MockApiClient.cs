using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using KIOT.Server.Core.Data.Api.Request;
using KIOT.Server.Core.Models.Data;
using KIOT.Server.Dto.Application.Appliances;
using KIOT.Server.Dto.Application.Customers.Appliances;
using KIOT.Server.Dto.Application.Customers.Data;

namespace KIOT.Server.Data.External.Api.Request
{
    internal sealed class MockApiClient : IApiClient
    {
        private readonly List<Customer> _customers = new List<Customer>
        {
            new Customer("0055_ALT-CM0102", Enumerable.Range(1, 10).Select(x => new Sensor("", 1, null, new ObservationTarget(10, "test"))))
        };

        public Task<bool> CustomerExists(CustomerCode code)
            => Task.FromResult(_customers.Any(c => c.CustomerId.Code == code.Code));

        public Task<IEnumerable<Customer>> GetAllCustomers()
            => Task.FromResult(_customers as IEnumerable<Customer>);

        public Task<IEnumerable<ApplianceActivityDto>> GetApplianceActivity(CustomerCode code)
            => Task.FromResult((IEnumerable<ApplianceActivityDto>)new List<ApplianceActivityDto>() { new ApplianceActivityDto() });

        public Task<(Appliance appliance, ApplianceType type)?> GetCustomerAppliance(CustomerCode code, int applianceId)
        {
            return Task.FromResult(((Appliance, ApplianceType)?)(new Appliance(123, new List<short?>(), new List<long>()), new ApplianceType(10, Enumerable.Empty<Appliance>())));
        }

        public Task<ObservedDataResponse> GetCustomerAppliances(CustomerCode code, TimeInterval timeInterval, ushort intervalOffset = 0)
            => Task.FromResult(new ObservedDataResponse(1, 1, "0055_ALT-CM0102", "UK", new List<ObservedData>() { new ObservedData(10, new List<long>(), new List<ApplianceType>(), new List<short?>())}));

        public Task<IEnumerable<ApplianceDto>> GetCustomerAppliances(CustomerCode code)
            => Task.FromResult((IEnumerable<ApplianceDto>) new List<ApplianceDto>() { new ApplianceDto() });

        public Task<IEnumerable<Customer>> GetResponseAsync(IGetCustomerRequest request)
            => Task.FromResult((IEnumerable<Customer>) new List<Customer>() { _customers.First() });

        public Task<ObservedData> GetResponseAsync(IGetObservedDataRequest request)
            => Task.FromResult(new ObservedData(10, new List<long>(), new List<ApplianceType>(), new List<short?>()));
    }
}
