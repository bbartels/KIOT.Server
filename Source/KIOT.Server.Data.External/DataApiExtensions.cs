using Microsoft.Extensions.DependencyInjection;

using KIOT.Server.Core.Data.Api;
using KIOT.Server.Core.Data.Api.Request;
using KIOT.Server.Data.External.Api;
using KIOT.Server.Data.External.Api.Json;
using KIOT.Server.Data.External.Api.Request;

namespace KIOT.Server.Data.External
{
    public static class DataApiExtensions
    {
        public static IServiceCollection RegisterApiServices(this IServiceCollection services)
        {
            //services.AddTransient<IApiClient, ExternalApiClient>();
            services.AddTransient<IApiClient, MockApiClient>();
            services.AddSingleton<IApiConfiguration, ExternalApiConfiguration>();
            services.AddSingleton<IRequestBuilder, ExternalRequestBuilder>();
            services.AddSingleton<IApiHttpClientAccessor, ExternalHttpClientAccessor>();
            services.AddSingleton<IJsonEntityParser, JsonEntityParser>();
            return services;
        }
    }
}
