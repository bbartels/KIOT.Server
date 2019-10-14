using System.Net;

namespace KIOT.Server.Core.Services
{
    public interface IHttpRequestService
    {
        string GetRemoteIpAddress(IPAddress address);
    }
}
