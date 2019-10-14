using System;

namespace KIOT.Server.Core.Models.Data
{
    public readonly struct SensorTimestamp
    {
        public long Ticks { get; }

        public DateTimeOffset Timestamp => DateTimeOffset.FromUnixTimeSeconds(Ticks);

        public SensorTimestamp(long ticks) => Ticks = ticks;

        public SensorTimestamp(DateTimeOffset offset) => Ticks = offset.ToUnixTimeSeconds();

        public SensorTimestamp(int daysOffsetNow) =>
            Ticks = DateTimeOffset.UtcNow.AddDays(daysOffsetNow).ToUnixTimeSeconds();
    }
}
