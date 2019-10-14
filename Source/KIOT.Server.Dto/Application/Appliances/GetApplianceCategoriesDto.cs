using System.Collections.Generic;

namespace KIOT.Server.Dto.Application.Appliances
{
    public class GetApplianceCategoriesDto
    {
        public IEnumerable<ApplianceCategoryDto> Categories { get; set; }
    }
}
