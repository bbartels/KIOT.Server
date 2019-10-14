namespace KIOT.Server.Dto.Application.Authentication
{
    public enum MobileOS
    {
        IOS,
        Android,
        Other
    }

    public class RegisterPushTokenDto
    {
        public string PushToken { get; set; }
        public string DeviceName { get; set; }
        public string InstallationId { get; set; }
        public MobileOS MobileOs { get; set; }
    }
}
