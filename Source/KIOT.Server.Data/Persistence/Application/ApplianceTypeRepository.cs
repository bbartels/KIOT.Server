using KIOT.Server.Core.Data.Persistence.Application;
using KIOT.Server.Core.Models.Application;

namespace KIOT.Server.Data.Persistence.Application
{
    internal class ApplianceTypeRepository : Repository<ApplianceType>, IApplianceTypeRepository
    {
        private KIOTContext KIOTContext => Context as KIOTContext;

        public ApplianceTypeRepository(KIOTContext context) : base(context) { }
    }
}
