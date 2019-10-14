using System;

using KIOT.Server.Dto.Application.Appliances;

namespace KIOT.Server.Dto.Application.Customers.Appliances
{
    public class ApplianceDto
    {
        public ushort ApplianceTypeId { get; set; }
        public int ApplianceId { get; set; }
        public string ApplianceName { get; set; }
        public DateTime LastActivity { get; set; }
        public ApplianceCategoryDto Category { get; set; }
    }
}
