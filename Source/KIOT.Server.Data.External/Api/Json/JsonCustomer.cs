using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

using KIOT.Server.Core.Models.Data;

namespace KIOT.Server.Data.External.Api.Json
{
    internal class JsonCustomer
    {
        [JsonProperty("Customer")]
        public string CustomerId { get; set; }

        [JsonProperty("Meters")]
        public IList<JsonMeter> Meters { get; set; }

        public Customer ToCustomer()
        {
            return new Customer(CustomerId, Meters.Select(x => x.ToSensor()));
        }
    }
}