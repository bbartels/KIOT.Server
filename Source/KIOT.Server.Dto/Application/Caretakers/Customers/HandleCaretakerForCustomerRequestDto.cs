using FluentValidation;

namespace KIOT.Server.Dto.Application.Caretakers.Customers
{
    public class HandleCaretakerForCustomerRequestDto
    {
        public bool AcceptRequest { get; set; }
    }

    public class HandleCaretakerForCustomerRequestDtoValidator : AbstractValidator<HandleCaretakerForCustomerRequestDto>
    {
        public HandleCaretakerForCustomerRequestDtoValidator()
        {
            RuleFor(x => x.AcceptRequest).NotNull();
        }
    }
}
