using System.Collections.Generic;

using KIOT.Server.Dto.Application.Tasks;

namespace KIOT.Server.Dto.Application.Customers.Tasks
{
    public class CustomerTasksDto
    {
        public IEnumerable<CustomerTaskDto> Tasks { get; set; }
    }
}
