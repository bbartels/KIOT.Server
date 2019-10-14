using System.Collections.Generic;
using KIOT.Server.Dto.Application.Customers;

namespace KIOT.Server.Dto.Application.Caretakers.Customers
{
    public class AssignedCustomersForCaretakerDto
    {
        public IEnumerable<CustomerInfoDto> Customers { get; set; }
    }
}
