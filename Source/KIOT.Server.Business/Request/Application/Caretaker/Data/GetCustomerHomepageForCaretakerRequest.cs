using System;
using System.Security.Claims;
using FluentValidation;
using KIOT.Server.Business.Response.Application.Caretakers;

namespace KIOT.Server.Business.Request.Application.Caretaker.Data
{
    public class GetCustomerHomepageForCaretakerRequest : AuthenticatedCaretakerRequest<CustomerHomepageForCaretakerResponse>
    {
        public Guid CustomerGuid { get; }

        public GetCustomerHomepageForCaretakerRequest(ClaimsIdentity identity, Guid customerGuid) : base(identity)
        {
            CustomerGuid = customerGuid;
        }
    }

    public class GetCustomerHomepageForCaretakerRequestValidator : 
        AuthenticatedCaretakerRequestValidator<GetCustomerHomepageForCaretakerRequest>
    {
        public GetCustomerHomepageForCaretakerRequestValidator()
        {
            RuleFor(x => x.CustomerGuid).NotEmpty();
        }
    }
}
