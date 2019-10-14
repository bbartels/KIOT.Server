using System.Collections.Generic;

using KIOT.Server.Core.Response;
using KIOT.Server.Dto.Application.Customers.Data;

namespace KIOT.Server.Business.Response.Application.Caretakers.Customers
{
    public class GetApplianceHistoryResponse : CommandResponse<CustomerDetailedPageDto>
    {
        public GetApplianceHistoryResponse(IEnumerable<Error> errors) : base(errors) { }

        public GetApplianceHistoryResponse(CustomerDetailedPageDto model) : base(model) { }
    }
}
