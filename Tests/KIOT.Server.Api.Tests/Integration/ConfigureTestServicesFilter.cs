using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace KIOT.Server.Api.Tests.Integration
{
    internal class ConfigureTestServicesFilter : IStartupConfigureServicesFilter
    {
        private readonly Action<IServiceCollection> _testServices;

        public ConfigureTestServicesFilter(Action<IServiceCollection> testServices)
        {
            _testServices = testServices ?? throw new ArgumentNullException(nameof(testServices));
        }

        public Action<IServiceCollection> ConfigureServices(Action<IServiceCollection> next)
        {
            return serviceCollection =>
            {
                next(serviceCollection);
                _testServices(serviceCollection);
            };
        }
    }
}
