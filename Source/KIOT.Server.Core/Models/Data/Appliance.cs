using System;
using System.Collections.Generic;
using System.Linq;

namespace KIOT.Server.Core.Models.Data
{
    public class Appliance
    {
        public uint ApplianceId { get; }
        public IList<short?> PowerConsumption { get; }
        private IList<long> Timestamps { get; }

        public Appliance(uint applianceId, IList<short?> powerConsumption, IList<long> timestamps)
        {
            ApplianceId = applianceId;
            PowerConsumption = powerConsumption;
            Timestamps = timestamps;
        }

        public double CalculateTotalAverage()
        {
            return PowerConsumption.Average(a => a ?? 0);
        }

        public IEnumerable<short> GetIntervalEveryNthPoint(int n)
        {
            var list = new List<short>();
            //TODO: Lowest common denominator??
            for (var i = 0; i < (Timestamps.Count / n); i++)
            {
                int sum = 0;
                for (var v = i * n; v < (i + 1) * (n); v++)
                {
                    sum += PowerConsumption[v] ?? 0;
                }

                list.Add((short)(sum / n));
            }

            return list;
        }

        public DateTime? ApplianceLastUsed()
        {
            foreach (var (value, index) in PowerConsumption.Reverse().Select((v, i) => (v, i)))
            {
                if (value != null && value > 1)
                {
                    return DateTimeOffset.FromUnixTimeSeconds(Timestamps[Timestamps.Count - index - 1]).DateTime;
                }
            }

            return null;
        }
    }
}
