using System.Security.Claims;
using FluentValidation;
using KIOT.Server.Core.Response;
using KIOT.Server.Dto.Application.Customers.Caretakers;

namespace KIOT.Server.Business.Request.Application.Customer.Caretakers
{
    public class AddCaretakerForCustomerRequest : AuthenticatedCustomerRequest<CommandResponse>
    {
        public string CaretakerUsername { get; }

        public AddCaretakerForCustomerRequest(AddCaretakerForCustomerDto dto, ClaimsIdentity customerIdentity) : base(customerIdentity)
        {
            CaretakerUsername = dto.CaretakerUsername;
        }
    }

    public class AddCaretakerForCustomerRequestValidator : AuthenticatedCustomerRequestValidator<AddCaretakerForCustomerRequest>
    {
        public AddCaretakerForCustomerRequestValidator()
        {
            RuleFor(cfc => cfc.CaretakerUsername).NotEmpty();
        }
    }
}
