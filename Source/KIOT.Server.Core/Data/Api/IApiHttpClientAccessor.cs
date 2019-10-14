using System.Net.Http;

namespace KIOT.Server.Core.Data.Api
{
    public interface IApiHttpClientAccessor
    {
        HttpClient Client { get; }
    }
}
