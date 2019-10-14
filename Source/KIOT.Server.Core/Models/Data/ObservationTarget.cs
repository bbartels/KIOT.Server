namespace KIOT.Server.Core.Models.Data
{
    public class ObservationTarget
    {
        public int ApplianceTypeId { get; }
        public string Name { get; }

        public ObservationTarget(int applianceTypeId, string name)
        {
            ApplianceTypeId = applianceTypeId;
            Name = name;
        }
    }
}
