using FluentValidation;

namespace KIOT.Server.Dto.Application.Authentication
{
    public class RegisterCustomerDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string CustomerCode { get; set; }
        public string PhoneNumber { get; set; }
    }

    public class RegisterCustomerDtoValidator : AbstractValidator<RegisterCustomerDto>
    {
        public RegisterCustomerDtoValidator()
        {
            RuleFor(c => c.FirstName).NotNull().Length(2, 32);
            RuleFor(c => c.LastName).NotNull().Length(2, 32);
            RuleFor(c => c.Password).NotNull().Length(6, 32);
            RuleFor(c => c.Email).NotNull().EmailAddress();
            RuleFor(c => c.Username).NotNull().Length(4, 32);
            RuleFor(c => c.CustomerCode).NotNull().Matches("^[a-zA-Z0-9]{4}_[a-zA-Z0-9-]{10}$");
            RuleFor(c => c.PhoneNumber).NotNull().Matches(@"^\+[1-9]{1}[0-9]{4,14}$");
        }
    }
}
