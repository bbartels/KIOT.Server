using System.Collections.Generic;

using KIOT.Server.Core.Response;
using KIOT.Server.Dto.Application.Caretakers;
using KIOT.Server.Dto.Application.Caretakers.Customers;

namespace KIOT.Server.Business.Response.Application.Caretakers
{
    public class GetPendingCaretakerRequestsResponse : CommandResponse<PendingCaretakerRequestsDto>
    {
        public GetPendingCaretakerRequestsResponse(IEnumerable<Error> errors) : base(errors) { }

        public GetPendingCaretakerRequestsResponse(PendingCaretakerRequestsDto model) : base(model) { }
    }
}
