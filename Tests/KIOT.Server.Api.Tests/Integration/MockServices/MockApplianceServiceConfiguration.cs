using KIOT.Server.Business.Abstractions.Services;

namespace KIOT.Server.Api.Tests.Integration.MockServices
{
    internal class MockApplianceServiceConfiguration : IApplianceServiceConfiguration
    {
        public int ApplianceCacheTimeout => 5 * 60;
        public int ActivityCacheTimeout => 5 * 60;
        public bool BackgroundJobsEnabled => false;
    }
}
