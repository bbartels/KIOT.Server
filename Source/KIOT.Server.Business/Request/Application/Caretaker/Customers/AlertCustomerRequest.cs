using System.Security.Claims;
using FluentValidation;

using KIOT.Server.Core.Response;
using KIOT.Server.Dto.Application.Caretakers;
using KIOT.Server.Dto.Application.Caretakers.Customers;

namespace KIOT.Server.Business.Request.Application.Caretaker.Customers
{
    public class AlertCustomerRequest : AuthenticatedCaretakerRequest<CommandResponse>
    {
        public AlertCustomerDto Dto { get; }

        public AlertCustomerRequest(AlertCustomerDto dto, ClaimsIdentity identity) : base(identity)
        {
            Dto = dto;
        }
    }

    public class AlertCustomerRequestValidator : AbstractValidator<AlertCustomerRequest>
    {
        public AlertCustomerRequestValidator()
        {
            RuleFor(x => x.Dto.Message).NotNull().NotEmpty();
            RuleFor(x => x.Dto.Username).NotNull().NotEmpty();
        }
    }
}
