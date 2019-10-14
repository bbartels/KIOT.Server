using System.Collections.Generic;
using KIOT.Server.Dto.Application.Customers;
using KIOT.Server.Dto.Data;

namespace KIOT.Server.Dto.Application.Caretakers.Data
{
    public class CustomerHomepageForCaretakerDto
    {
        public CustomerInfoDto Customer { get; set; }
        public IEnumerable<PowerUsageOverIntervalDto> PowerUsageRatios { get; set; }
    }
}
