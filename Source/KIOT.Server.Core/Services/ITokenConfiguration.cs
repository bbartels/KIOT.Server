using System;

namespace KIOT.Server.Core.Services
{
    public interface ITokenConfiguration
    {
        string Issuer { get; }
        string Audience { get; }
        string Secret { get; }
        byte[] SigningKey { get; }
        DateTime IssuedAt { get; }
        DateTime ExpiresAt { get; }
        TimeSpan ExpiresIn { get; }
    }
}
