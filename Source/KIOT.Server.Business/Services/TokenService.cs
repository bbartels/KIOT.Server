using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using JWT;
using JWT.Algorithms;
using JWT.Builder;

using KIOT.Server.Core.Models.Application;
using KIOT.Server.Core.Services;

namespace KIOT.Server.Business.Services
{
    internal class TokenService : ITokenService
    {
        private const int ByteCount = 48;
        private const string IdentityIdClaimTypeConst = "identity_id";
        private readonly ITokenConfiguration _tokenConfiguration;

        public TokenService(ITokenConfiguration configuration) => _tokenConfiguration = configuration;

        public string GenerateRefreshToken()
        {
            using (var rngProvider = new RNGCryptoServiceProvider())
            {
                var numbers = new byte[ByteCount];
                rngProvider.GetBytes(numbers);
                return Convert.ToBase64String(numbers);
            }
        }

        public (string token, TimeSpan expiresIn) GenerateAccessToken(User user, IEnumerable<Claim> claims)
        {
            var builder = new JwtBuilder()
                .WithAlgorithm(new HMACSHA256Algorithm())
                .WithSecret(_tokenConfiguration.Secret)
                .Issuer(_tokenConfiguration.Issuer)
                .Audience(_tokenConfiguration.Audience)
                .IssuedAt(_tokenConfiguration.IssuedAt)
                .ExpirationTime(_tokenConfiguration.ExpiresAt)
                .AddClaim(IdentityIdClaimTypeConst, user.IdentityId)
                .AddClaim("guid", user.Guid)
                .AddClaim("role", GetUserRoles())
                .Subject(user.Username)
                .GivenName(user.FirstName)
                .FamilyName(user.LastName);

            string GetUserRoles()
            {
                string role;
                switch (user)
                {
                    case Customer _: { role = "Customer"; } break;
                    case Caretaker _: { role = "Caretaker"; } break;
                    default: { throw new ArgumentException($"Invalid User: {user.GetType()}"); }
                }

                return role;
            }

            foreach (var claim in claims)
            {
                builder.AddClaim(claim.Type, claim.Value);
            }

            var token = builder.Build();
            Debug.Assert(VerifyAccessToken(token));

            return (token, _tokenConfiguration.ExpiresIn);
        }

        public bool VerifyAccessToken(string token)
        {
            try
            {
                new JwtBuilder()
                    .WithSecret(_tokenConfiguration.Secret)
                    .MustVerifySignature()
                    .Decode(token);
            }

            catch (TokenExpiredException) { return false; }
            catch (SignatureVerificationException) { return false; }

            return true;
        }

        public IEnumerable<Claim> GetUserClaims(string accessToken)
        {
            return new JwtSecurityTokenHandler().ReadJwtToken(accessToken).Claims;
        }

        public string IdentityIdClaimType => IdentityIdClaimTypeConst;
    }
}
