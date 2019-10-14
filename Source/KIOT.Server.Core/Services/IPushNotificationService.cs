using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;
using KIOT.Server.Core.Models.Application;

namespace KIOT.Server.Core.Services
{
    public class PushNotification
    {
        public string PushToken { get; private set; }
        public string Title { get; private set; }
        public string Message { get; private set; }
        public IDictionary<string, string> Data { get; }

        public PushNotification(string pushToken, string message, string title, IDictionary<string, string> data = null)
        {
            PushToken = pushToken;
            Message = message;
            Title = title;
            Data = data ?? new Dictionary<string, string>();
        }

        public string ToJson()
        {
            var dict = new Dictionary<string, object>
            {
                { "to", $"ExponentPushToken[{PushToken}]"},
                { "title", Title },
                { "body", Message },
                { "sound", "default" },
                { "data", Data }
            };

            return JsonConvert.SerializeObject(dict);
        }
    }

    public class NotificationResponse
    {
        public string Status { get; private set; }
        public string Message { get; private set; }
        public string Error { get; private set; }
    }

    public interface IPushNotificationService
    {
        Task SendNotificationsAsync(IEnumerable<PushNotification> notifications);
        Task SendNotificationsAsync(User user, string title, string message);
    }
}
