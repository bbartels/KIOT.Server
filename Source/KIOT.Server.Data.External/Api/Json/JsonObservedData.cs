using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

using KIOT.Server.Core.Models.Data;

namespace KIOT.Server.Data.External.Api.Json
{
    internal class JsonObservedData
    {
        [JsonProperty("TimeUnitId")]
        public short TimeUnitId { get; set; }

        [JsonProperty("TimeStamps")]
        public IList<long> Timestamps { get; set; }

        [JsonProperty("ApplianceTypes")]
        public IList<JsonApplianceType> ApplianceTypes { get; set; }

        [JsonProperty("RootPowers")]
        public IList<double?> RootPowers { get; set; }

        public ObservedData ToObservedData()
        {
            return new ObservedData(TimeUnitId, Timestamps, ApplianceTypes.Select(at => at.ToApplianceType(Timestamps)).ToList(),
                RootPowers.Select(p => (short?)p).ToList());
        }
    }
}
