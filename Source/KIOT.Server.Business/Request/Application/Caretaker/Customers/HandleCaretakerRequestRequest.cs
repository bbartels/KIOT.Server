using System;
using System.Security.Claims;
using FluentValidation;

using KIOT.Server.Core.Response;

namespace KIOT.Server.Business.Request.Application.Caretaker.Customers
{
    public class HandleCaretakerRequestRequest : AuthenticatedCaretakerRequest<CommandResponse>
    {
        public Guid RequestId { get; }
        public bool AcceptRequest { get; }

        public HandleCaretakerRequestRequest(Guid requestId, bool acceptRequest, ClaimsIdentity identity) : base(identity)
        {
            RequestId = requestId;
            AcceptRequest = acceptRequest;
        }
    }

    public class HandleCaretakerRequestRequestValidator :
        AuthenticatedCaretakerRequestValidator<HandleCaretakerRequestRequest>
    {
        public HandleCaretakerRequestRequestValidator()
        {
            RuleFor(x => x.RequestId).Must(x => x != Guid.Empty);
        }
    }
}
