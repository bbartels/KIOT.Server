using System.Collections.Generic;
using System.Linq;

namespace KIOT.Server.Core.Models.Data
{
    public class ObservedDataResponse
    {
        public short DataType { get; set; }
        public short Version { get; set; }
        public CustomerCode Customer { get; set; }
        public string  TimeZone { get; set; }
        public ObservedData Data { get; set; }

        public ObservedDataResponse(short dataType, short version, string customerId, string timeZone,
            IEnumerable<ObservedData> data)
        {
            DataType = dataType;
            Version = version;
            Customer = new CustomerCode(customerId);
            TimeZone = timeZone;
            Data = data?.First();
        }
    }
}
