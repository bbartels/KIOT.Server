using System.Security.Claims;
using FluentValidation;

using KIOT.Server.Business.Response.Application.Customers.Appliances;

namespace KIOT.Server.Business.Request.Application.Customer.Appliances
{
    public class GetApplianceCategoriesRequest : AuthenticatedCustomerRequest<GetApplianceCategoriesResponse>
    {
        public GetApplianceCategoriesRequest(ClaimsIdentity identity) : base(identity) { }
    }

    public class GetApplianceCategoriesRequestValidator : AbstractValidator<GetApplianceCategoriesRequest>
    {
        public GetApplianceCategoriesRequestValidator() { }
    }
}
