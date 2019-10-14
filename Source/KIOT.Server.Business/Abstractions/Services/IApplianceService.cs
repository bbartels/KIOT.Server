using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using KIOT.Server.Core.Models.Application;
using KIOT.Server.Core.Models.Data;
using KIOT.Server.Dto.Application.Appliances;
using KIOT.Server.Dto.Application.Customers.Appliances;
using KIOT.Server.Dto.Application.Customers.Data;

namespace KIOT.Server.Business.Abstractions.Services
{
    internal interface IApplianceService
    {
        Task<IDictionary<int, CustomerAppliance>> GetApplianceInfo(Guid customerGuid);

        void AddApplianceInfo(IEnumerable<ApplianceDto> appliances,
            IDictionary<int, CustomerAppliance> applianceInfo);

        Task<IEnumerable<ApplianceDto>> GetCustomerAppliances(Guid customerGuid);
        Task<ApplianceDto> GetCustomerAppliance(Guid customerGuid, int applianceId);
        Task<ObservedData> GetCustomerApplianceHistory(Guid customerGuid, TimeInterval time, ushort intervalOffset);
        Task CacheCustomerApplianceHistory(Guid customerGuid);
        Task<IEnumerable<ApplianceActivityDto>> GetCustomerAppliancesActivity(Guid customerGuid);
    }
}
