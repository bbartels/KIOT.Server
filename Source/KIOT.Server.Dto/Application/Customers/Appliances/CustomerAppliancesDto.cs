using System.Collections.Generic;

namespace KIOT.Server.Dto.Application.Customers.Appliances
{
    public class CustomerAppliancesDto
    {
        public IEnumerable<ApplianceDto> Appliances { get; set; }
    }
}
