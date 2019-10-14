using System.Collections.Generic;

namespace KIOT.Server.Core.Models.Application
{
    public class ApplianceType : BaseEntity
    {
        public int ApplianceTypeId { get; private set; }
        public string Name { get; private set; }

        public ICollection<CustomerAppliance> CustomerAppliances { get; set; }

        public ApplianceType(int applianceTypeId, string name)
        {
            ApplianceTypeId = applianceTypeId;
            Name = name;
        }

        public void SetName(string name)
        {
            if (Name != name) { Name = name; }
        }
    }
}
