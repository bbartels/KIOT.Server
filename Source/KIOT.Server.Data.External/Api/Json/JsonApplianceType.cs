using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

using KIOT.Server.Core.Models.Data;

namespace KIOT.Server.Data.External.Api.Json
{
    internal class JsonApplianceType
    {
        [JsonProperty("ApplianceTypeId")]
        public ushort ApplianceTypeId { get; set; }

        [JsonProperty("Appliances")]
        public IEnumerable<JsonAppliance> Appliances { get; set; }

        public ApplianceType ToApplianceType(IList<long> timestamps)
        {
            return new ApplianceType(ApplianceTypeId, Appliances.Select(ap => ap.ToAppliance(timestamps)));
        }
    }
}
