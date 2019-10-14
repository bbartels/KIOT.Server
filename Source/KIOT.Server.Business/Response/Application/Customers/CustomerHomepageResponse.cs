using System.Collections.Generic;

using KIOT.Server.Core.Response;
using KIOT.Server.Dto.Application.Customers;
using KIOT.Server.Dto.Application.Customers.Data;

namespace KIOT.Server.Business.Response.Application.Customers
{
    public class CustomerHomepageResponse : CommandResponse<CustomerHomepageDto>
    {
        public CustomerHomepageResponse(IEnumerable<Error> errors) : base(errors) { }

        public CustomerHomepageResponse(CustomerHomepageDto model) : base(model) { }
    }
}
