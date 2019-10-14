using System.Collections.Generic;

namespace KIOT.Server.Core.Models.Data
{
    public class CustomerDataResponse
    {
        public uint Total { get; }
        public uint Offset { get; }
        public IEnumerable<Customer> Customers { get; }

        public CustomerDataResponse(uint total, uint offset, IEnumerable<Customer> customers)
        {
            Total = total;
            Offset = offset;
            Customers = customers;
        }
    }
}
