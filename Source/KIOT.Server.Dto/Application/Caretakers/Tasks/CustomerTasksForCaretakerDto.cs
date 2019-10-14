using System.Collections.Generic;

using KIOT.Server.Dto.Application.Customers;
using KIOT.Server.Dto.Application.Tasks;

namespace KIOT.Server.Dto.Application.Caretakers.Tasks
{
    public class CustomerTasksForCaretakerDto
    {
        public CustomerInfoDto Customer { get; set; }
        public IEnumerable<CustomerTaskDto> Tasks { get; set; }
    }
}
