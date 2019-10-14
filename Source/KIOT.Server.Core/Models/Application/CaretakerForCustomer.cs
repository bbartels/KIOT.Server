namespace KIOT.Server.Core.Models.Application
{
    public class CaretakerForCustomer : BaseEntity
    {
        public int CustomerId { get; private set; }
        public int CaretakerId { get; private set; }
        public Customer Customer { get; private set; }
        public Caretaker Caretaker { get; private set; }

        public CaretakerForCustomer(int caretakerId, int customerId)
        {
            CustomerId = customerId;
            CaretakerId = caretakerId;
        }
    }
}
