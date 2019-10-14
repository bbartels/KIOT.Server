using System.Net;
using KIOT.Server.Core.Services;

namespace KIOT.Server.Business.Services
{
    internal class HttpRequestService : IHttpRequestService
    {
        public string GetRemoteIpAddress(IPAddress address) { return address.ToString(); }
    }
}
