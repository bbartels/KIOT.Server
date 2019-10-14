using System;
using System.Collections.Generic;

using KIOT.Server.Core.Response;

namespace KIOT.Server.Business.Response.Application
{
    public class ExchangeRefreshTokenResponse : CommandResponse
    {
        public string AccessToken { get; }
        public string RefreshToken { get; }
        public TimeSpan ExpiresIn { get; }

        public ExchangeRefreshTokenResponse(IEnumerable<Error> errors, string message) : base(errors) { Message = message; } 
        public ExchangeRefreshTokenResponse(string accessToken, TimeSpan expiresIn, string refreshToken, string message)
        {
            AccessToken = accessToken;
            RefreshToken = refreshToken;
            ExpiresIn = expiresIn;
            Message = message;
        }
    }
}
