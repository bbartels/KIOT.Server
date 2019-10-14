using System;
using System.Security.Claims;
using FluentValidation;

using KIOT.Server.Business.Response.Application.Caretakers.Customers;

namespace KIOT.Server.Business.Request.Application.Caretaker.Customers
{
    public class GetCustomerApplianceActivityRequest : AuthenticatedCaretakerRequest<GetCustomerApplianceActivityResponse>
    {
        public Guid CustomerGuid { get; set; }

        public GetCustomerApplianceActivityRequest(Guid customerGuid, ClaimsIdentity identity) : base(identity)
        {
            CustomerGuid = customerGuid;
        }
    }

    public class GetCustomerApplianceActivityRequestValidator : AbstractValidator<GetCustomerApplianceActivityRequest>
    {
        public GetCustomerApplianceActivityRequestValidator()
        {
            RuleFor(x => x.CustomerGuid).Must(x => Guid.Empty != x);
        }
    }
}
