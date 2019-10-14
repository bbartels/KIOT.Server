using System;
using System.Collections.Generic;

using KIOT.Server.Core.Response;

namespace KIOT.Server.Business.Response.Application
{
    public class LoginUserResponse : CommandResponse
    {
        public string AccessToken { get; }
        public string RefreshToken { get; }
        public TimeSpan ExpiresIn { get; }

        public LoginUserResponse(IEnumerable<Error> errors, string message) : base(errors) { Message = message; } 
        public LoginUserResponse(string accessToken, TimeSpan expiresIn, string refreshToken, string message)
        {
            AccessToken = accessToken;
            RefreshToken = refreshToken;
            ExpiresIn = expiresIn;
            Message = message;
        }
    }
}
