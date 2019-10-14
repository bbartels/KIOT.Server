using FluentValidation;

namespace KIOT.Server.Dto.Application.Authentication
{
    public class LoginUserDto
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class LoginUserDtoValidator : AbstractValidator<LoginUserDto>
    {
        public LoginUserDtoValidator()
        {
            RuleFor(u => u.Username).NotEmpty();
            RuleFor(u => u.Password).NotEmpty();
        }
    }
}
