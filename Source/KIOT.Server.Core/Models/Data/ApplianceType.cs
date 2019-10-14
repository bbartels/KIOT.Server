using System.Collections.Generic;

namespace KIOT.Server.Core.Models.Data
{
    public class ApplianceType
    {
        public ushort ApplianceTypeId { get; }
        public IEnumerable<Appliance> Appliances { get; set; }

        public ApplianceType(ushort applianceTypeId, IEnumerable<Appliance> appliances)
        {
            ApplianceTypeId = applianceTypeId;
            Appliances = appliances;
        }
    }
}
