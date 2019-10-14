using System;
using System.Net;

namespace KIOT.Server.Core.Models.Application
{
    public class RefreshToken : BaseEntity
    {
        public string Token { get; private set; }
        public string UserId { get; private set; }
        public IPAddress RemoteAddress { get; private set; }
        public DateTime ExpiresOn { get; private set; }
        public bool IsActive => DateTime.UtcNow <= ExpiresOn;

        public RefreshToken() { }
        public RefreshToken(string token, string userId, DateTime expiresOn, IPAddress remoteAddress)
        {
            Token = token;
            UserId = userId;
            ExpiresOn = expiresOn;
            RemoteAddress = remoteAddress;
        }
    }
}
