using System;
using System.Collections.Generic;

using KIOT.Server.Core.Data.Api;
using KIOT.Server.Core.Services;

namespace KIOT.Server.Data.External.Api
{
    internal class ExternalApiConfiguration : IApiConfiguration
    {
        private static string BaseUrlCache = "-removed-";
        private static string ProtocolCache = "https";
        private static string VersionCache = "0.1";

        private static readonly IDictionary<ApiRequestType, string> RequestPathCache = new Dictionary<ApiRequestType, string>
        {
            { ApiRequestType.GetCustomers, "-removed-" },
            { ApiRequestType.GetObservedData, "-removed-" }
        };

        public ExternalApiConfiguration(IApiKeyService keyService)
            => BasicAuthenticationHeader = keyService.ApiKey;

        public string BaseUrl => BaseUrlCache ?? ReadBaseUrl();
        public string Protocol => ProtocolCache ?? ReadApiProtocol();
        public string Version => VersionCache ?? ReadApiVersion();

        public string BasicAuthenticationHeader { get; }

        public string GetRequestPath(ApiRequestType request)
        {
            return RequestPathCache.ContainsKey(request) ? RequestPathCache[request] : ReadApiRequestPath(request);
        }

        private string ReadBaseUrl()
            => throw new NotImplementedException();

        private string ReadApiProtocol()
            => throw new NotImplementedException();

        private string ReadApiVersion()
            => throw new NotImplementedException();

        private string ReadApiRequestPath(ApiRequestType request)
            => throw new NotImplementedException();
    }
}
