using System;
using System.Security.Claims;
using FluentValidation;

using KIOT.Server.Business.Response.Application.Caretakers.Tasks;

namespace KIOT.Server.Business.Request.Application.Caretaker.Tasks
{
    public class GetCustomerTasksRequest : AuthenticatedCaretakerRequest<GetCustomerTasksResponse>
    {
        public Guid CustomerGuid { get; }

        public GetCustomerTasksRequest(Guid customerGuid, ClaimsIdentity identity) : base(identity)
        {
            CustomerGuid = customerGuid;
        }
    }

    public class GetCustomerTasksRequestValidator : AbstractValidator<GetCustomerTasksRequest>
    {
        public GetCustomerTasksRequestValidator()
        {
            RuleFor(x => x.CustomerGuid).Must(x => x != Guid.Empty);
        }
    }
}
