using System.Security.Claims;
using FluentValidation;
using MediatR;

using KIOT.Server.Business.Response.Application.Caretakers;

namespace KIOT.Server.Business.Request.Application.Caretaker.Customers
{
    public class GetPendingCaretakerRequestsRequest : AuthenticatedCaretakerRequest<GetPendingCaretakerRequestsResponse>
    {
        public GetPendingCaretakerRequestsRequest(ClaimsIdentity identity) : base(identity) { }
    }

    public class GetPendingCaretakerRequestsRequestValidator :
        AuthenticatedCaretakerRequestValidator<GetPendingCaretakerRequestsRequest>
    {
        public GetPendingCaretakerRequestsRequestValidator() { }
    }
}
