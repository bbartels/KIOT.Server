using KIOT.Server.Business.Abstractions.Services;

namespace KIOT.Server.Business.Services
{
    internal class ApplianceServiceConfiguration : IApplianceServiceConfiguration
    {
        public int ApplianceCacheTimeout => 5 * 60;
        public int ActivityCacheTimeout => 60;
        public bool BackgroundJobsEnabled => true;
    }
}
