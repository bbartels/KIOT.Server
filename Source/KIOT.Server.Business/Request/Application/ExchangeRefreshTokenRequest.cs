using System.IdentityModel.Tokens.Jwt;
using FluentValidation;
using MediatR;

using KIOT.Server.Business.Response.Application;
using KIOT.Server.Dto.Application.Authentication;

namespace KIOT.Server.Business.Request.Application
{
    public class ExchangeRefreshTokenRequest : IRequest<ExchangeRefreshTokenResponse>
    {
        public string AccessToken { get; }
        public string RefreshToken { get; }
        public string RemoteAddress { get; }


        public ExchangeRefreshTokenRequest(ExchangeRefreshTokenDto dto, string remoteAddress)
        {
            AccessToken = dto.AccessToken;
            RefreshToken = dto.RefreshToken;
            RemoteAddress = remoteAddress;
        }
    }

    public class ExchangeRefreshTokenRequestValidator : AbstractValidator<ExchangeRefreshTokenRequest>
    {
        public ExchangeRefreshTokenRequestValidator()
        {
            RuleFor(x => x.AccessToken).NotEmpty().Must(x => new JwtSecurityTokenHandler().CanReadToken(x));
            RuleFor(x => x.RefreshToken).NotEmpty();
            RuleFor(x => x.RemoteAddress).NotEmpty();
        }
    }
}
