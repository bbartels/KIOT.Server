using System.Collections.Generic;
using KIOT.Server.Dto.Application.Caretakers;

namespace KIOT.Server.Dto.Application.Customers.Caretakers
{
    public class AssignedCaretakersForCustomerDto
    {
        public IEnumerable<CaretakerInfoDto> Caretakers { get; set; }
    }
}
