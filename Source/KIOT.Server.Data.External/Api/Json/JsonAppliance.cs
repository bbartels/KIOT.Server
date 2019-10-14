using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

using KIOT.Server.Core.Models.Data;

namespace KIOT.Server.Data.External.Api.Json
{
    internal class JsonAppliance
    {
        [JsonProperty("ApplianceId")]
        public uint ApplianceId { get; set; }

        [JsonProperty("Powers")]
        public IList<double?> Powers { get; set; }

        public Appliance ToAppliance(IList<long> timestamps)
        {
            return new Appliance(ApplianceId, Powers.Select(x => (short?)x).ToList(), timestamps);
        }
    }
}
