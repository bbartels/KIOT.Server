using System.Collections.Generic;

using KIOT.Server.Core.Models.Application;
using KIOT.Server.Core.Response;

namespace KIOT.Server.Core.Data.Response.Caretaker
{
    public class GetPendingCaretakerRequestsResponse : DataResponse<IEnumerable<CaretakerForCustomerRequest>>
    {
        public GetPendingCaretakerRequestsResponse(
            IEnumerable<CaretakerForCustomerRequest> entity) : base(entity) { }

        public GetPendingCaretakerRequestsResponse(
            IEnumerable<CaretakerForCustomerRequest> entity, Error error) : base(entity, error) { }
    }
}
