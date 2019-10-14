using System.Net.NetworkInformation;

namespace KIOT.Server.Core.Models.Data
{
    public class Sensor
    {
        public PhysicalAddress MacAddress { get; }
        public SensorTimestamp StartTimestamp { get; }
        public SensorTimestamp? EndTimestamp { get; }
        public ObservationTarget ObservationTarget { get; }

        public Sensor(string macAddress, long startTicks, long? endTicks, ObservationTarget target)
        {
            MacAddress = PhysicalAddress.Parse(macAddress);
            StartTimestamp = new SensorTimestamp(startTicks);
            EndTimestamp = endTicks == null ? (SensorTimestamp?)null : new SensorTimestamp((long)endTicks);
            ObservationTarget = target;
        }
    }
}
