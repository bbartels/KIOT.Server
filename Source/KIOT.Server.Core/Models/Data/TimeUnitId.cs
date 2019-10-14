using System;
using System.Collections.Generic;

namespace KIOT.Server.Core.Models.Data
{
    public enum TimeUnit : byte
    {
        Second = 10,
        Minute = 20,
        HalfHour = 25,
        Hour = 30,
        Day = 40,
        Month = 50,
        Year = 60
    }

    public readonly struct TimeUnitId
    {
        private static readonly Dictionary<short, TimeSpan> TimeUnitIdMap = new Dictionary<short, TimeSpan>
        {
            { 10, TimeSpan.FromSeconds(1) },
            { 20, TimeSpan.FromMinutes(1) },
            { 25, TimeSpan.FromMinutes(30) },
            { 30, TimeSpan.FromHours(1) },
            { 40, TimeSpan.FromDays(1) },
            { 50, TimeSpan.FromDays(30) },
            { 60, TimeSpan.FromDays(365) },
        };

        public short Id { get; }
        public TimeSpan Duration => TimeUnitIdMap[Id];

        public TimeUnitId(TimeUnit unit)
        {
            if (!Enum.IsDefined(typeof(TimeUnit), unit))
            {
                throw new ArgumentException("Undefined Enum");
            }

            Id = (byte)unit;
        }

        public TimeUnitId(short id)
        {
            if (!TimeUnitIdMap.ContainsKey(id))
            {
                throw new ArgumentException($"No { nameof(TimeUnitId) } " +
                    $"with value { id } is registered.");
            }

            Id = id;
        }
    }
}
