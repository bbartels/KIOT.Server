using System;

namespace KIOT.Server.Core.Models.Application
{
    public class PushToken : BaseEntity
    {
        public string Token { get; private set; }
        public DateTime RegisteredAt { get; private set; } = DateTime.UtcNow;
        public DateTime? EndedAt { get; private set; }
        public bool IsValid => !EndedAt.HasValue;


        public int MobileDeviceId { get; private set; }
        public MobileDevice MobileDevice { get; private set; }

        public int UserId { get; private set; }
        public User User { get; private set; }

        public PushToken(MobileDevice device, string token) : this(device.Id, token)
        {
            MobileDevice = device;
        }

        public PushToken(int mobileDeviceId, string token)
        {
            MobileDeviceId = mobileDeviceId;

            if (string.IsNullOrWhiteSpace(token))
            {
                throw new ArgumentException($"Invalid PushToken: {token}.");
            }

            Token = token;
        }

        public void InvalidateToken()
        {
            if (IsValid) { EndedAt = DateTime.UtcNow; }
        }
    }
}
