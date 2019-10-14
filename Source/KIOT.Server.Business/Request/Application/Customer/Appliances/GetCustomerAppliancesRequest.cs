using System.Security.Claims;
using FluentValidation;
using KIOT.Server.Business.Response.Application.Customers;

namespace KIOT.Server.Business.Request.Application.Customer.Appliances
{
    public class GetCustomerAppliancesRequest : AuthenticatedCustomerRequest<GetCustomerAppliancesResponse>
    {
        public GetCustomerAppliancesRequest(ClaimsIdentity identity) : base(identity)
        {

        }
    }

    public class GetCustomerAppliancesRequestValidator : AbstractValidator<GetCustomerAppliancesRequest>
    {
        public GetCustomerAppliancesRequestValidator()
        {
        }
    }
}
