using Newtonsoft.Json;

using KIOT.Server.Core.Models.Data;

namespace KIOT.Server.Data.External.Api.Json
{
    internal class JsonObservationTarget
    {
        [JsonProperty("ApplianceTypeId")]
        public int ApplianceTypeId { get; set; }

        [JsonProperty("Name")]
        public string Name { get; set; }

        public ObservationTarget ToObservationTarget()
        {
            return new ObservationTarget(ApplianceTypeId, Name);
        }
    }
}
