using System;
using System.Security.Claims;
using FluentValidation;
using KIOT.Server.Core.Response;

namespace KIOT.Server.Business.Request.Application.Customer.Caretakers
{
    public class DeleteCaretakerForCustomerRequest : AuthenticatedCustomerRequest<CommandResponse>
    {
        public Guid CaretakerGuid { get; }

        public DeleteCaretakerForCustomerRequest(ClaimsIdentity identity, Guid caretakerGuid) : base(identity)
        {
            CaretakerGuid = caretakerGuid;
        }
    }

    public class DeleteCaretakerForCustomerRequestValidator : AuthenticatedCustomerRequestValidator<DeleteCaretakerForCustomerRequest>
    {
        public DeleteCaretakerForCustomerRequestValidator()
        {
            RuleFor(x => x.CaretakerGuid).NotEmpty();
        }
    }
}
