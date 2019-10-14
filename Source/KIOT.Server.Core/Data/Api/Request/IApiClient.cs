using System.Collections.Generic;
using System.Threading.Tasks;

using KIOT.Server.Core.Models.Data;
using KIOT.Server.Dto.Application.Appliances;
using KIOT.Server.Dto.Application.Customers.Appliances;
using KIOT.Server.Dto.Application.Customers.Data;
using ApplianceType = KIOT.Server.Core.Models.Data.ApplianceType;
using Customer = KIOT.Server.Core.Models.Data.Customer;

namespace KIOT.Server.Core.Data.Api.Request
{
    public interface IApiClient
    {
        Task<IEnumerable<Customer>> GetResponseAsync(IGetCustomerRequest request); 
        Task<ObservedData> GetResponseAsync(IGetObservedDataRequest request);
        Task<bool> CustomerExists(CustomerCode code);
        Task<ObservedDataResponse> GetCustomerAppliances(CustomerCode code, TimeInterval timeInterval, ushort intervalOffset = 0);
        Task<IEnumerable<ApplianceDto>> GetCustomerAppliances(CustomerCode code);
        Task<(Appliance appliance, ApplianceType type)?> GetCustomerAppliance(CustomerCode code, int applianceId);
        Task<IEnumerable<Customer>> GetAllCustomers();
        Task<IEnumerable<ApplianceActivityDto>> GetApplianceActivity(CustomerCode code);
    }
}
