using System.Collections.Generic;

using KIOT.Server.Core.Response;
using KIOT.Server.Dto.Application.Caretakers;
using KIOT.Server.Dto.Application.Caretakers.Customers;

namespace KIOT.Server.Business.Response.Application.Caretakers
{
    public class GetAssignedCustomersResponse : CommandResponse<AssignedCustomersForCaretakerDto>
    {
        public GetAssignedCustomersResponse(IEnumerable<Error> errors) : base(errors) { }

        public GetAssignedCustomersResponse(AssignedCustomersForCaretakerDto model) : base(model) { }
    }
}
