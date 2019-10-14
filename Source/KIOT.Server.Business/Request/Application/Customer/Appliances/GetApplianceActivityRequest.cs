using System.Security.Claims;
using FluentValidation;

using KIOT.Server.Business.Response.Application.Customers.Appliances;

namespace KIOT.Server.Business.Request.Application.Customer.Appliances
{
    public class GetApplianceActivityRequest : AuthenticatedCustomerRequest<GetApplianceActivityResponse>
    {
        public GetApplianceActivityRequest(ClaimsIdentity identity) : base(identity) { }
    }

    public class GetApplianceActivityRequestValidator : AbstractValidator<GetApplianceActivityRequest> { }
}
