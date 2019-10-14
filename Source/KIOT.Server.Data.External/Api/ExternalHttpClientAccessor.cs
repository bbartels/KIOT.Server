using System.Net.Http;

using KIOT.Server.Core.Data.Api;

namespace KIOT.Server.Data.External.Api
{
    internal class ExternalHttpClientAccessor : IApiHttpClientAccessor
    {
        private const string AuthorizationParameterKey = "Authorization";
        public HttpClient Client { get; } = new HttpClient();

        public ExternalHttpClientAccessor(IApiConfiguration apiConfig)
        {
            Client.DefaultRequestHeaders.TryAddWithoutValidation(AuthorizationParameterKey, apiConfig.BasicAuthenticationHeader);
        }
    }
}
