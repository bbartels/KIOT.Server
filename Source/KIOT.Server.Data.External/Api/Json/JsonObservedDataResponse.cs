using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

using KIOT.Server.Core.Models.Data;

namespace KIOT.Server.Data.External.Api.Json
{
    internal class JsonObservedDataResponse
    {
        [JsonProperty("DataType")]
        public long DataType { get; set; }

        [JsonProperty("Version")]
        public long Version { get; set; }

        [JsonProperty("Customer")]
        public string Customer { get; set; }

        [JsonProperty("TimeZone")]
        public string TimeZone { get; set; }

        [JsonProperty("Data")]
        public IEnumerable<JsonObservedData> Data { get; set; }

        public ObservedDataResponse ToObservedDataResponse()
        {
            return new ObservedDataResponse((short)DataType, (short)Version, Customer, TimeZone, Data?.Select(od => od.ToObservedData()));
        }
    }
}
