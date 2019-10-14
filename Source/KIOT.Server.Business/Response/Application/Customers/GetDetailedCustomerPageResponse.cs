using System.Collections.Generic;

using KIOT.Server.Core.Response;
using KIOT.Server.Dto.Application.Customers;
using KIOT.Server.Dto.Application.Customers.Data;

namespace KIOT.Server.Business.Response.Application.Customers
{
    public class GetDetailedCustomerPageResponse : CommandResponse<CustomerDetailedPageDto>
    {
        public GetDetailedCustomerPageResponse(IEnumerable<Error> errors) : base(errors) { }

        public GetDetailedCustomerPageResponse(CustomerDetailedPageDto model) : base(model) { }
    }
}
