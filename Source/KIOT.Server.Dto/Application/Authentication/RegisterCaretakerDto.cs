using FluentValidation;

namespace KIOT.Server.Dto.Application.Authentication
{
    public class RegisterCaretakerDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string PhoneNumber { get; set; }
    }

    public class RegisterCaretakerDtoValidator : AbstractValidator<RegisterCaretakerDto>
    {
        public RegisterCaretakerDtoValidator()
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
