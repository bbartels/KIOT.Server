using System.Security.Claims;
using KIOT.Server.Business.Response.Application.Customers;

namespace KIOT.Server.Business.Request.Application.Customer.Caretakers
{
    public class GetCaretakersForCustomerRequest : AuthenticatedCustomerRequest<GetCaretakersForCustomerResponse>
    {
        public GetCaretakersForCustomerRequest(ClaimsIdentity identity) : base(identity) { }
    }

    public class GetCaretakersForCustomerRequestValidator :
        AuthenticatedCustomerRequestValidator<GetCaretakersForCustomerRequest>
    {
        public GetCaretakersForCustomerRequestValidator() { }
    }
}
