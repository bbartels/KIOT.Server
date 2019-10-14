using FluentValidation;

namespace KIOT.Server.Dto.Application.Authentication
{
    public class ExchangeRefreshTokenDto
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }

    public class ExchangeRefreshTokenDtoValidator : AbstractValidator<ExchangeRefreshTokenDto>
    {
        public ExchangeRefreshTokenDtoValidator()
        {
            RuleFor(x => x.AccessToken).NotEmpty();
            RuleFor(x => x.RefreshToken).NotEmpty();
        }
    }
}
