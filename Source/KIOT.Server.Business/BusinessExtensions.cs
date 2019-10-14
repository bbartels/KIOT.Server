using Microsoft.Extensions.DependencyInjection;

using KIOT.Server.Business.Abstractions.Jobs;
using KIOT.Server.Business.Abstractions.Services;
using KIOT.Server.Business.Jobs;
using KIOT.Server.Business.Services;
using KIOT.Server.Core.Services;

namespace KIOT.Server.Business
{
    public static class BusinessExtensions
    {
        public static IServiceCollection RegisterBusinessServices(this IServiceCollection services)
        {
            services.AddSingleton<ITokenService, TokenService>();
            services.AddSingleton<IHttpRequestService, HttpRequestService>();
            services.AddSingleton<IExpoNotificationApiClient, ExpoNotificationApiClient>();
            services.AddSingleton<IPushNotificationService, ExpoPushNotificationService>();
            services.AddScoped<IApplianceService, ApplianceService>();
            services.AddScoped<ICustomerService, CustomerService>();
            services.AddScoped<IBackgroundTaskScheduler, BackgroundTaskScheduler>();
            services.AddSingleton<IApplianceServiceConfiguration, ApplianceServiceConfiguration>();

            services.AddScoped<IBackgroundJob<CacheCustomersForCaretakerRequest>, CacheCustomersForCaretakerJob>();
            services.AddScoped<IBackgroundJob<CacheCustomerApplianceHistoryRequest>, CacheCustomerApplianceHistoryJob>();

            return services;
        }
    }
}
