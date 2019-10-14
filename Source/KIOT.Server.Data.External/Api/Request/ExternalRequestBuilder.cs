using System.Collections.Concurrent;

using KIOT.Server.Core.Data.Api;
using KIOT.Server.Core.Data.Api.Request;

namespace KIOT.Server.Data.External.Api.Request
{
    internal class ExternalRequestBuilder : IRequestBuilder
    {
        private static readonly ConcurrentDictionary<ApiRequestType, string> CachedUrls = new ConcurrentDictionary<ApiRequestType, string>();

        private readonly IApiConfiguration _apiConfig;

        public ExternalRequestBuilder(IApiConfiguration configReader)
        {
            _apiConfig = configReader;
        }

        public string BuildRequest(IApiRequest request)
        {
            if (CachedUrls.ContainsKey(request.Type)) { return BuildFullUrl(); }

            var url = $"{ _apiConfig.Protocol}://{ _apiConfig.BaseUrl }/{ _apiConfig.Version }/{ _apiConfig.GetRequestPath(request.Type) }?";
            CachedUrls.TryAdd(request.Type, url);

            string BuildFullUrl() => CachedUrls[request.Type] + request.BuildQueryString();

            return BuildFullUrl();
        }
    }
}
