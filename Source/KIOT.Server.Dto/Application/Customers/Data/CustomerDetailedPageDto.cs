using System;
using System.Collections.Generic;

using KIOT.Server.Dto.Data;

namespace KIOT.Server.Dto.Application.Customers.Data
{
    public class CustomerDetailedPageDto
    {
        public DateTimeOffset StartTime { get; set; }
        public DateTimeOffset StartTimeInterval { get; set; }
        public DateTimeOffset EndTimeInterval { get; set; }
        public DateTimeOffset EndTime { get; set; }
        public IEnumerable<ApplianceUsageDto> Appliances { get; set; }
        public long DataPointEvery { get; set; }
    }
}
