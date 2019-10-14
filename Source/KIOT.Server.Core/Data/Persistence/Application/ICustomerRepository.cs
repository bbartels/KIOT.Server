using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using KIOT.Server.Core.Models.Data;
using KIOT.Server.Core.Response;
using Customer = KIOT.Server.Core.Models.Application.Customer;

namespace KIOT.Server.Core.Data.Persistence.Application
{
    public interface ICustomerRepository : IRepository<Customer>
    {
        Task<CustomerCode> GetCustomerCode(Guid customerId);
        Task<IDictionary<int, string>> GetAppliancesAliasMap(Guid customerId);
        Task<IDictionary<int, (int id, string name)>> GetAppliancesCategoryMap(Guid customerId);
        Task<DataResponse> SetAliasForCustomerAppliance(Guid customerGuid, int applianceId, string alias);
        Task<DataResponse<bool>> IsApplianceRegisteredWithCustomer(Guid customerGuid, int applianceId);
    }
}
