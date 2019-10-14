using System;
using System.Collections.Generic;
using System.Security.Claims;
using KIOT.Server.Core.Models.Application;

namespace KIOT.Server.Core.Services
{
    public interface ITokenService
    {
        string GenerateRefreshToken();
        (string token, TimeSpan expiresIn) GenerateAccessToken(User user, IEnumerable<Claim> claims);
        bool VerifyAccessToken(string token);
        IEnumerable<Claim> GetUserClaims(string accessToken);
        string IdentityIdClaimType { get; }
    }
}
