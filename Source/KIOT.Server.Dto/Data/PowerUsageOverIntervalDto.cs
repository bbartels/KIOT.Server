namespace KIOT.Server.Dto.Data
{
    public enum PowerUsageOverIntervalState : byte
    {
        NotEnoughData,
        NoCurrentData,
        Success
    }

    public enum TimePeriod : byte
    {
        Year,
        Month,
        Week
    }

    public class PowerUsageOverIntervalDto
    {
        public TimePeriod TimePeriod { get; set; }
        public PowerUsageOverIntervalState State { get; set; }
        public float? Ratio { get; set; }
        public int? CurrentUsage { get; set; }
        public int? AverageUsage { get; set; }
    }
}
