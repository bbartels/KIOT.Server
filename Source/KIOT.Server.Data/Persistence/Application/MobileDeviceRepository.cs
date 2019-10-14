using KIOT.Server.Core.Data.Persistence.Application;
using KIOT.Server.Core.Models.Application;

namespace KIOT.Server.Data.Persistence.Application
{
    internal class MobileDeviceRepository : Repository<MobileDevice>, IMobileDeviceRepository
    {
        private KIOTContext KIOTContext => Context as KIOTContext;

        public MobileDeviceRepository(KIOTContext context) : base(context) { }
    }
}
