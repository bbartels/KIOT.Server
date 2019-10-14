using System.Collections.Generic;
using KIOT.Server.Dto.Data;

namespace KIOT.Server.Dto.Application.Customers.Data
{
    public class CustomerHomepageDto
    {
        public CustomerInfoDto Customer { get; set; }
        public IEnumerable<PowerUsageOverIntervalDto> PowerUsageRatios { get; set; }
    }
}
