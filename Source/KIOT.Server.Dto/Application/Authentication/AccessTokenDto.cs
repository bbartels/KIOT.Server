namespace KIOT.Server.Dto.Application.Authentication
{
    public class AccessTokenDto
    {
        public string AccessToken { get; set; }
        public int ExpiresIn { get; set; }
        public string RefreshToken { get; set; }
    }
}
