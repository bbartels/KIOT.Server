using System.Net.Http;
using System.Threading.Tasks;

namespace KIOT.Server.Core.Services
{
    public interface IExpoNotificationApiClient
    {
        Task<HttpResponseMessage> SendAsync(string json);
    }
}
