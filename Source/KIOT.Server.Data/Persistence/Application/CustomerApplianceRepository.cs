
using KIOT.Server.Core.Data.Persistence.Application;
using KIOT.Server.Core.Models.Application;

namespace KIOT.Server.Data.Persistence.Application
{
    internal class CustomerApplianceRepository : Repository<CustomerAppliance>, ICustomerApplianceRepository
    {
        private KIOTContext KIOTContext => Context as KIOTContext;

        public CustomerApplianceRepository(KIOTContext context) : base(context) { }
    }
}
