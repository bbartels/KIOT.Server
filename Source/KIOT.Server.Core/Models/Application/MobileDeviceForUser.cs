using System.Threading;

namespace KIOT.Server.Core.Models.Application
{
    public class MobileDeviceForUser : BaseEntity
    {
        public int UserId { get; private set; }
        public User User { get; private set; }

        public int MobileDeviceId { get; private set; }
        public MobileDevice MobileDevice { get; private set; }

        public MobileDeviceForUser(int userId, int mobileDeviceId)
        {
            UserId = userId;
            MobileDeviceId = mobileDeviceId;
        }

        public MobileDeviceForUser(User user, MobileDevice mobileDevice)
        {
            User = user;
            MobileDevice = mobileDevice;
        }
    }
}
