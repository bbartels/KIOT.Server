using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

using KIOT.Server.Core.Models.Application;
using KIOT.Server.Core.Services;

namespace KIOT.Server.Business.Services
{
    internal class ExpoPushNotificationService : IPushNotificationService
    {
        private readonly IExpoNotificationApiClient _client;

        public ExpoPushNotificationService(IExpoNotificationApiClient client)
        {
            _client = client;
        }

        public async Task SendNotificationsAsync(IEnumerable<PushNotification> notifications)
        {
            await _client.SendAsync(JsonConvert.SerializeObject(notifications.Select(n => n.ToJson())));
        }

        public async Task SendNotificationsAsync(User user, string title, string message)
        {
            var notifications = user.GetValidPushTokens().Select(x => new PushNotification(x.Token, message, title));
            await SendNotificationsAsync(notifications);
        }
    }
}
