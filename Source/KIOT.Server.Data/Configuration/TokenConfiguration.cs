using System;
using System.Text;
using KIOT.Server.Core.Services;

namespace KIOT.Server.Data.Configuration
{
    internal class TokenConfiguration : ITokenConfiguration
    {
        private static string SecretCache = "SuperSecretSecret";
        private static string IssuerCache = "KIOT";
        private static int ValidForMinutesCache = 15;

        public string Issuer => IssuerCache ?? ReadIssuer();

        public string Secret => SecretCache ?? ReadSecret();
        public byte[] SigningKey => Encoding.ASCII.GetBytes(Secret);

        public string Audience => Issuer;
        public TimeSpan ExpiresIn => TimeSpan.FromMinutes(ValidForMinutesCache);
        public DateTime IssuedAt => DateTime.UtcNow;
        public DateTime ExpiresAt => IssuedAt.Add(ExpiresIn);

        private string ReadSecret() { throw new NotImplementedException(); }
        private string ReadIssuer() { throw new NotImplementedException(); }
    }
}
