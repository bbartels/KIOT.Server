using System.Collections.Generic;

using KIOT.Server.Core.Response;
using KIOT.Server.Dto.Application.Appliances;

namespace KIOT.Server.Business.Response.Application.Caretakers.Customers
{
    public class GetCustomerApplianceActivityResponse : CommandResponse<IEnumerable<ApplianceActivityDto>>
    {
        public GetCustomerApplianceActivityResponse(IEnumerable<Error> errors) : base(errors) { }
        public GetCustomerApplianceActivityResponse(IEnumerable<ApplianceActivityDto> model) : base(model) { }
    }
}
