using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

using KIOT.Server.Core.Models.Data;

namespace KIOT.Server.Data.External.Api.Json
{
    internal class JsonCustomerResponse
    {
        [JsonProperty("Total")]
        public uint Total { get; set; }

        [JsonProperty("Offset")]
        public uint Offset { get; set; }

        [JsonProperty("Customers")]
        public IList<JsonCustomer> Customers { get; set; }

        public CustomerDataResponse ToCustomerDataResponse()
        {
            return new CustomerDataResponse(Total, Offset, Customers.Select(x => x.ToCustomer()));
        }
    }
}
