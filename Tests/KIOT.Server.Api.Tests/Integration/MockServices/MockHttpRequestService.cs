using System.Net;

using KIOT.Server.Core.Services;

namespace KIOT.Server.Api.Tests.Integration.MockServices
{
    internal class MockHttpRequestService : IHttpRequestService
    {
        public string GetRemoteIpAddress(IPAddress address) { return "127.0.0.1"; }
    }
}
