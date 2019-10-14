using FluentValidation;
using MediatR;

using KIOT.Server.Core.Response;
using KIOT.Server.Dto.Application.Authentication;

namespace KIOT.Server.Business.Request.Application
{
    public class RegisterCaretakerRequest : IRequest<CommandResponse>
    {
        public string FirstName { get; }
        public string LastName { get; }
        public string Username { get; }
        public string Email { get; }
        public string Password { get; }
        public string PhoneNumber { get; }

        public RegisterCaretakerRequest(RegisterCaretakerDto dto)
        {
            FirstName = dto.FirstName;
            LastName = dto.LastName;
            Email = dto.Email;
            Username = dto.Username;
            Password = dto.Password;
            PhoneNumber = dto.PhoneNumber;
        }
    }

    public class RegisterCaretakerRequestValidator : AbstractValidator<RegisterCaretakerRequest>
    {
        public RegisterCaretakerRequestValidator()
        {
            RuleFor(x => x.FirstName).NotNull().Length(2, 32);
            RuleFor(x => x.LastName).NotNull().Length(2, 32);
            RuleFor(x => x.Password).NotNull().Length(6, 32);
            RuleFor(x => x.Email).NotNull().EmailAddress();
            RuleFor(x => x.Username).NotNull().Length(4, 32);
            RuleFor(c => c.PhoneNumber).NotNull().Matches(@"^\+[1-9]{1}[0-9]{4,14}$");
        }
    }
}
