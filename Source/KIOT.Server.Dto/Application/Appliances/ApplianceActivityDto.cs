using System;

namespace KIOT.Server.Dto.Application.Appliances
{
    public class ApplianceActivityDto
    {
        public int ApplianceId { get; set; }
        public DateTime LastActivity { get; set; }
    }
}
