using System;
using KIOT.Server.Dto.Application.Customers;

namespace KIOT.Server.Dto.Application.Caretakers.Customers
{
    public class CaretakerForCustomerRequestDto
    {
        public Guid RequestId { get; set; }
        public CustomerInfoDto Customer { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
