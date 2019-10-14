using System.Security.Claims;

using KIOT.Server.Business.Response.Application.Customers;

namespace KIOT.Server.Business.Request.Application.Customer.Data
{
    public class GetCustomerHomepageRequest : AuthenticatedCustomerRequest<CustomerHomepageResponse>
    {
        public GetCustomerHomepageRequest(ClaimsIdentity identity) : base(identity) { }
    }

    public class GetCustomerHomepageRequestValidator : AuthenticatedCustomerRequestValidator<GetCustomerHomepageRequest>
    {
        public GetCustomerHomepageRequestValidator() { }
    }
}
