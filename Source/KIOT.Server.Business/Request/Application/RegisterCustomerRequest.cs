using FluentValidation;
using MediatR;

using KIOT.Server.Core.Models.Data;
using KIOT.Server.Core.Response;
using KIOT.Server.Dto.Application.Authentication;

namespace KIOT.Server.Business.Request.Application
{
    public class RegisterCustomerRequest : IRequest<CommandResponse>
    {
        public string FirstName { get; }
        public string LastName { get; }
        public string Username { get; }
        public string Email { get; }
        public string Password { get; }
        public string PhoneNumber { get; }
        public CustomerCode CustomerCode { get; }

        public RegisterCustomerRequest(RegisterCustomerDto dto)
        {
            FirstName = dto.FirstName;
            LastName = dto.LastName;
            Email = dto.Email;
            Username = dto.Username;
            Password = dto.Password;
            CustomerCode = new CustomerCode(dto.CustomerCode);
            PhoneNumber = dto.PhoneNumber;
        }
    }

    public class RegisterCustomerRequestValidator : AbstractValidator<RegisterCustomerRequest>
    {
        public RegisterCustomerRequestValidator()
        {
            RuleFor(x => x.FirstName).NotNull().Length(2, 32);
            RuleFor(x => x.LastName).NotNull().Length(2, 32);
            RuleFor(x => x.Password).NotNull().Length(6, 32);
            RuleFor(x => x.Email).NotNull().EmailAddress();
            RuleFor(x => x.Username).NotNull().Length(4, 32);
            RuleFor(c => c.CustomerCode.Code).NotNull().Matches("^[a-zA-Z0-9]{4}_[a-zA-Z0-9-]{10}$");
            RuleFor(c => c.PhoneNumber).NotNull().Matches(@"^\+[1-9]{1}[0-9]{4,14}$");
        }
    }
}
