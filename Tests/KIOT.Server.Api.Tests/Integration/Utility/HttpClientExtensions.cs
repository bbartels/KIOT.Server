using System.Net.Http;
using System.Net.Http.Headers;

using KIOT.Server.Dto.Application.Authentication;

namespace KIOT.Server.Api.Tests.Integration.Utility
{
    internal static class HttpClientExtensions
    {
        public static void AddAuthorization(this HttpClient client, AccessTokenDto dto)
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", dto.AccessToken);
        }

        public static void AddAuthorization(this AccessTokenDto dto, HttpClient client)
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", dto.AccessToken);
        }
    }
}
