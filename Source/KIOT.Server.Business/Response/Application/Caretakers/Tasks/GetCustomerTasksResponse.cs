using System.Collections.Generic;

using KIOT.Server.Core.Response;
using KIOT.Server.Dto.Application.Caretakers.Tasks;

namespace KIOT.Server.Business.Response.Application.Caretakers.Tasks
{
    public class GetCustomerTasksResponse : CommandResponse<CustomerTasksForCaretakerDto>
    {
        public GetCustomerTasksResponse(IEnumerable<Error> errors) : base(errors) { }

        public GetCustomerTasksResponse(CustomerTasksForCaretakerDto model) : base(model) { }
    }
}
