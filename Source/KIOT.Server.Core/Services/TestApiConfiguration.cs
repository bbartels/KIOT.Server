using System;

using KIOT.Server.Core.Data.Api;

namespace KIOT.Server.Core.Services
{
    public class TestApiConfiguration : IApiConfiguration
    {
        public string BaseUrl => "-removed-";
        public string Protocol => "https";
        public string Version => "0.1";
        public string BasicAuthenticationHeader => Environment.GetEnvironmentVariable("ExternalApiKey");

        public string GetRequestPath(ApiRequestType request)
        {
            return request == ApiRequestType.GetCustomers ? "-removed-" : "-removed-";
        }
    }
}
