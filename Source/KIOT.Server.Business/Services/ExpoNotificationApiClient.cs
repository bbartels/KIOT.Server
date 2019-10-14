using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net.Http.Headers;

using KIOT.Server.Core.Services;

namespace KIOT.Server.Business.Services
{
    internal class ExpoNotificationApiClient : IExpoNotificationApiClient
    {
        private static HttpClient Client { get; }
        private const string HostUrl = "https://exp.host/--/api/v2/push/send";

        static ExpoNotificationApiClient()
        {
            var handler = new HttpClientHandler
            {
                AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip,
            };
            Client = new HttpClient(handler);
            Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<HttpResponseMessage> SendAsync(string json)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, HostUrl)
            {
                Content = new StringContent(json)
            };
            request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            var response = await Client.SendAsync(request);
            return null;
        }
    }
}
