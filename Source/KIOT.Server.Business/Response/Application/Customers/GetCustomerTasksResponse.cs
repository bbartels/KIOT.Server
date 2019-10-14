using System.Collections.Generic;

using KIOT.Server.Core.Response;
using KIOT.Server.Dto.Application.Customers.Tasks;

namespace KIOT.Server.Business.Response.Application.Customers
{
    public class GetCustomerTasksResponse : CommandResponse<CustomerTasksDto>
    {
        public GetCustomerTasksResponse(IEnumerable<Error> errors) : base(errors) { }

        public GetCustomerTasksResponse(CustomerTasksDto model) : base(model) { }
    }
}
