using System;

namespace KIOT.Server.Core.Models.Application
{
    public class CaretakerForCustomerRequest : BaseEntity
    {
        public int CustomerId { get; private set; }
        public int CaretakerId { get; private set; }
        public bool Handled { get; private set; } = false;
        public DateTime Timestamp { get; private set; } = DateTime.UtcNow;

        public Customer Customer { get; private set; }
        public Caretaker Caretaker { get; private set; }

        private CaretakerForCustomerRequest() { }

        public CaretakerForCustomerRequest(int caretakerId, int customerId)
        {
            CaretakerId = caretakerId;
            CustomerId = customerId;
        }
    }
}
