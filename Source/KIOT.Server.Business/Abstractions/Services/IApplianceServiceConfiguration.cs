namespace KIOT.Server.Business.Abstractions.Services
{
    internal interface IApplianceServiceConfiguration
    {
        int ApplianceCacheTimeout { get; }
        int ActivityCacheTimeout { get; }
        bool BackgroundJobsEnabled { get; }
    }
}
