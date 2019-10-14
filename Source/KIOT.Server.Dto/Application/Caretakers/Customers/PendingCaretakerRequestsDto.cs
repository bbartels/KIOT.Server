using System.Collections.Generic;

namespace KIOT.Server.Dto.Application.Caretakers.Customers
{
    public class PendingCaretakerRequestsDto
    {
        public IEnumerable<CaretakerForCustomerRequestDto> CaretakerRequests { get; set; }
    }
}
