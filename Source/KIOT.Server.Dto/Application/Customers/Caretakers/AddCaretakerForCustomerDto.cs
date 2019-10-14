using FluentValidation;

namespace KIOT.Server.Dto.Application.Customers.Caretakers
{
    public class AddCaretakerForCustomerDto
    {
        public string CaretakerUsername { get; set; }
    }

    public class AddCaretakerForCustomerDtoValidator : AbstractValidator<AddCaretakerForCustomerDto>
    {
        public AddCaretakerForCustomerDtoValidator()
        {
            RuleFor(cfc => cfc.CaretakerUsername).NotEmpty();
        }
    }
}
