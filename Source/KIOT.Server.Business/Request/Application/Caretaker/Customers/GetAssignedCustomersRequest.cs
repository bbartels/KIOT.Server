using System;
using System.Security.Claims;
using FluentValidation;
using MediatR;

using KIOT.Server.Business.Response.Application.Caretakers;

namespace KIOT.Server.Business.Request.Application.Caretaker.Customers
{
    public class GetAssignedCustomersRequest : AuthenticatedCaretakerRequest<GetAssignedCustomersResponse>
    {
        public GetAssignedCustomersRequest(ClaimsIdentity identity) : base(identity) { }
    }

    public class GetAssignedCustomerRequestValidator :
        AuthenticatedCaretakerRequestValidator<GetAssignedCustomersRequest>
    {
        public GetAssignedCustomerRequestValidator() { }
    }
}
