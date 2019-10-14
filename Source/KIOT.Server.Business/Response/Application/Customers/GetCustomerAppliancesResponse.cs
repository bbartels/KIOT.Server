using System.Collections.Generic;

using KIOT.Server.Core.Response;
using KIOT.Server.Dto.Application.Customers;
using KIOT.Server.Dto.Application.Customers.Appliances;

namespace KIOT.Server.Business.Response.Application.Customers
{
    public class GetCustomerAppliancesResponse : CommandResponse<CustomerAppliancesDto>
    {
        public GetCustomerAppliancesResponse(IEnumerable<Error> errors) : base(errors) { }

        public GetCustomerAppliancesResponse(CustomerAppliancesDto model) : base(model) { }
    }
}
