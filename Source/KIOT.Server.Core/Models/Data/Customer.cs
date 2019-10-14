using System.Collections.Generic;

namespace KIOT.Server.Core.Models.Data
{
    public class Customer
    {
        public CustomerCode CustomerId { get; }
        public IEnumerable<Sensor> Sensors { get; }

        public Customer(string customerId, IEnumerable<Sensor> sensors)
        {
            CustomerId = new CustomerCode(customerId);
            Sensors = sensors;
        }
    }
}
