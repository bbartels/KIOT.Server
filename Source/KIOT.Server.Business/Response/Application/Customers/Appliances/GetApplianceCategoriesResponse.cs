using System.Collections.Generic;

using KIOT.Server.Core.Response;
using KIOT.Server.Dto.Application.Appliances;

namespace KIOT.Server.Business.Response.Application.Customers.Appliances
{
    public class GetApplianceCategoriesResponse : CommandResponse<GetApplianceCategoriesDto>
    {
        public GetApplianceCategoriesResponse(IEnumerable<Error> errors) : base(errors) { }

        public GetApplianceCategoriesResponse(GetApplianceCategoriesDto model) : base(model) { }
    }
}
