using System.Collections.Generic;

using KIOT.Server.Core.Response;
using KIOT.Server.Dto.Application.Appliances;

namespace KIOT.Server.Business.Response.Application.Customers.Appliances
{
    public class GetApplianceActivityResponse : CommandResponse<IEnumerable<ApplianceActivityDto>>
    {
        public GetApplianceActivityResponse(IEnumerable<Error> errors) : base(errors) { }

        public GetApplianceActivityResponse(IEnumerable<ApplianceActivityDto> model) : base(model) { }
    }
}
