using FluentValidation;
using MediatR;

using KIOT.Server.Business.Response.Application;
using KIOT.Server.Dto.Application;
using KIOT.Server.Dto.Application.Authentication;

namespace KIOT.Server.Business.Request.Application
{
    public class LoginUserRequest : IRequest<LoginUserResponse>
    {
        public string Username { get; }
        public string Password { get; }
        public string RemoteAddress { get; }

        public LoginUserRequest(LoginUserDto dto, string remoteAddress)
        {
            Username = dto.Username;
            Password = dto.Password;
            RemoteAddress = remoteAddress;
        }
    }

    public class LoginUserRequestValidator : AbstractValidator<LoginUserRequest>
    {
        public LoginUserRequestValidator()
        {
            RuleFor(r => r.Username).NotNull().NotEmpty();
            RuleFor(r => r.Password).NotNull().NotEmpty();
        }
    }
}
