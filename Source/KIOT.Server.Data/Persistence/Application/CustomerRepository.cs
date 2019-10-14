using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

using KIOT.Server.Core.Data.Persistence.Application;
using KIOT.Server.Core.Models.Data;
using KIOT.Server.Core.Response;
using Customer = KIOT.Server.Core.Models.Application.Customer;

namespace KIOT.Server.Data.Persistence.Application
{
    internal class CustomerRepository : Repository<Customer>, ICustomerRepository
    {
        private KIOTContext KIOTContext => Context as KIOTContext;

        public CustomerRepository(KIOTContext context) : base(context) { }

        public Task<CustomerCode> GetCustomerCode(Guid customerId)
        {
            return KIOTContext.Customers.Where(c => c.Guid == customerId)
                .Select(c => new CustomerCode(c.Code)).SingleOrDefaultAsync();
        }

        public async Task<IDictionary<int, string>> GetAppliancesAliasMap(Guid customerId)
        {
            return (await KIOTContext.Customers.Include(c => c.Appliances).ThenInclude(a => a.ApplianceType)
                .FirstOrDefaultAsync(c => c.Guid == customerId)).Appliances.ToDictionary(x => x.ApplianceId, x => x.Name);
        }

        public async Task<IDictionary<int, (int id, string name)>> GetAppliancesCategoryMap(Guid customerId)
        {
            return (await KIOTContext.Customers.Include(c => c.Appliances).ThenInclude(a => a.Category)
                .FirstOrDefaultAsync(c => c.Guid == customerId)).Appliances.Where(x => x.CategoryId != null)
                .ToDictionary(x => x.ApplianceId, x => (x.CategoryId ?? 0, x.Category.Name));
        }

        public async Task<DataResponse> SetAliasForCustomerAppliance(Guid customerGuid, int applianceId, string alias)
        {
            var appliance = await KIOTContext.CustomerAppliances.Include(ca => ca.Customer)
                .SingleOrDefaultAsync(x => x.Customer.Guid == customerGuid && x.ApplianceId == applianceId);

            if (appliance == null)
            {
                return new DataResponse(new Error("InvalidAppliance", "Could not find given appliance."));
            }

            try
            {
                appliance.Alias = alias;
                await KIOTContext.SaveChangesAsync();
                return new DataResponse();
            }

            catch (DbUpdateException)
            {
                return new DataResponse(new Error("ServerError", "Could not update alias of given appliance."));
            }
        }

        public async Task<DataResponse<bool>> IsApplianceRegisteredWithCustomer(Guid customerGuid, int applianceId)
        {
            var applianceExists = await KIOTContext.CustomerAppliances.Include(ca => ca.Customer)
                .AnyAsync(ca => ca.ApplianceId == applianceId && ca.Customer.Guid == customerGuid);

            return new DataResponse<bool>(applianceExists);
        }
    }
}
