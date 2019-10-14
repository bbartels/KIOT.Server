using System;
using System.Collections.Generic;
using System.Linq;
using KIOT.Server.Dto.Data;

namespace KIOT.Server.Core.Models.Data
{
    public class ObservedData
    {
        public TimeUnitId TimeUnitId { get; }
        public IList<long> Timestamps { get; }
        public IList<ApplianceType> ApplianceTypes { get; }
        public IList<short?> TotalPowerConsumption { get; }

        public ObservedData(short id, IList<long> timestamps, IList<ApplianceType> applianceTypes,
                            IList<short?> totalPowerDraw)
        {
            TimeUnitId = new TimeUnitId(id);
            Timestamps = timestamps;
            ApplianceTypes = applianceTypes;
            TotalPowerConsumption = totalPowerDraw;
        }

        public PowerUsageOverIntervalDto PowerUsageOverMedianOfIntervals(int interval, int includeDays = 0)
        {
            var trimmedPowerUsage = TotalPowerConsumption.Skip(TotalPowerConsumption.Count - includeDays)
                .SkipWhile(x => x == null).ToList();
            var trimmedRatio = trimmedPowerUsage.Count / (float) includeDays;

            if (trimmedRatio < 0.7) { return new PowerUsageOverIntervalDto { State = PowerUsageOverIntervalState.NotEnoughData }; }

            var itemsInInterval = trimmedPowerUsage.Count / interval;
            interval = (int)(interval * trimmedRatio);
            var powerOverInterval = new short?[itemsInInterval];
            var averageUsages = new int?[interval];

            for (int intIndex = 0; intIndex < interval; intIndex++)
            {
                for (int i = 0; i < itemsInInterval; i++)
                {
                    powerOverInterval[i] = trimmedPowerUsage[intIndex * itemsInInterval + i];
                }

                averageUsages[intIndex] = (int?)powerOverInterval.Average(x => x);
            }

            var average = averageUsages.Where(x => x != null)
                .OrderBy(x => x).Skip(averageUsages.Length / 2).First();

            if (averageUsages[averageUsages.Length - 1] != null)
            {
                var currentUsage = (float) (averageUsages[averageUsages.Length - 1] ?? 0);
                var averageUsage = average ?? 0;

                return new PowerUsageOverIntervalDto
                {
                    CurrentUsage = averageUsages[averageUsages.Length - 1],
                    AverageUsage = averageUsage,
                    State = PowerUsageOverIntervalState.Success,
                    Ratio = (float) Math.Round(1 / (averageUsage / currentUsage), 2)
                };
            }

            return new PowerUsageOverIntervalDto { State = PowerUsageOverIntervalState.NoCurrentData };

        }
    }
}
