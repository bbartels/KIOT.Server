using Newtonsoft.Json;

using KIOT.Server.Core.Models.Data;

namespace KIOT.Server.Data.External.Api.Json
{
    internal class JsonMeter
    {
        [JsonProperty("Mac")]
        public string Mac { get; set; }

        [JsonProperty("StartTimestamp")]
        public long StartTimestamp { get; set; }

        [JsonProperty("EndTimestamp")]
        public long? EndTimestamp { get; set; }

        [JsonProperty("ObservationTarget")]
        public JsonObservationTarget ObservationTarget { get; set; }

        [JsonProperty("ChannelId")]
        public long ChannelId { get; set; }

        public Sensor ToSensor()
        {
            return new Sensor(Mac, StartTimestamp, EndTimestamp, ObservationTarget.ToObservationTarget());
        }
    }
}
