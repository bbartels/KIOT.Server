using System.Collections.Generic;

using KIOT.Server.Dto.Application.Customers.Appliances;

namespace KIOT.Server.Dto.Data
{
    public class ApplianceUsageDto
    {
        public ApplianceDto Appliance { get; set; }

        public short? TotalAverageUsage { get; set; }
        public short? CurrentAverageUsage { get; set; }
        public IEnumerable<short> UsagePoints { get; set; }
    }
}
