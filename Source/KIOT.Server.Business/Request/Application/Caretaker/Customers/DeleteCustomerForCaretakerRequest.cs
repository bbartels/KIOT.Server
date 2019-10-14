using System;
using System.Security.Claims;
using FluentValidation;
using MediatR;

using KIOT.Server.Core.Response;

namespace KIOT.Server.Business.Request.Application.Caretaker.Customers
{
    public class DeleteCustomerForCaretakerRequest : AuthenticatedCaretakerRequest<CommandResponse>
    {
        public Guid CustomerGuid { get; }

        public DeleteCustomerForCaretakerRequest(ClaimsIdentity identity, Guid customerGuid) : base(identity)
        {
            CustomerGuid = customerGuid;
        }
    }

    public class DeleteCustomerForCaretakerRequestValidator :
        AuthenticatedCaretakerRequestValidator<DeleteCustomerForCaretakerRequest>
    {
        public DeleteCustomerForCaretakerRequestValidator()
        {
            RuleFor(x => x.CustomerGuid).Must(x => x != Guid.Empty);
        }
    }
}
