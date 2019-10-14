using System;
using System.Collections.Generic;

namespace KIOT.Server.Core.Models.Application
{
    public enum MobileOS
    {
        IOS,
        Android,
        Other
    }

    public class MobileDevice : BaseEntity
    {
        public string DeviceName { get; private set; }
        public MobileOS MobileOS { get; private set; }
        public string InstallationId { get; private set; }
        public DateTime LastLoginAt { get; private set; } = DateTime.UtcNow;

        public ICollection<PushToken> PushTokens { get; private set; }
        public ICollection<MobileDeviceForUser> UsedBy { get; private set; }

        public MobileDevice(string deviceName, MobileOS mobileOS, string installationId)
        {
            DeviceName = deviceName;
            MobileOS = mobileOS;
            InstallationId = installationId;
        }

        public void InvalidateRegisteredPushTokens()
        {
            if (PushTokens == null) { return; }

            foreach (var token in PushTokens)
            {
                token.InvalidateToken();
            }
        }
    }
}
