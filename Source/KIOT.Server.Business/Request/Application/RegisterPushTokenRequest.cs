using System;
using System.Security.Claims;
using FluentValidation;

using KIOT.Server.Core.Response;
using KIOT.Server.Dto.Application;
using KIOT.Server.Dto.Application.Authentication;

namespace KIOT.Server.Business.Request.Application
{
    public class RegisterPushTokenRequest : AuthenticatedRequest<CommandResponse>
    {
        public Guid UserGuid => Identity.GetGuid();
        public RegisterPushTokenDto PushTokenDto { get; }
        public RegisterPushTokenRequest(RegisterPushTokenDto pushTokenDto, ClaimsIdentity identity) : base(identity)
        {
            PushTokenDto = pushTokenDto;
        }
    }

    public class RegisterPushTokenRequestValidator : AbstractValidator<RegisterPushTokenRequest>
    {
        public RegisterPushTokenRequestValidator()
        {
            RuleFor(x => x.PushTokenDto).NotNull();
            RuleFor(x => x.PushTokenDto.PushToken).NotNull().NotEmpty();
        }
    }
}
