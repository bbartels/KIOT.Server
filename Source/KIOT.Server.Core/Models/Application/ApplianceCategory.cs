using System.Collections.Generic;
using System.Linq;

namespace KIOT.Server.Core.Models.Application
{
    public class ApplianceCategory : BaseEntity
    {
        public int CustomerId { get; private set; }
        public Customer Customer { get; private set; }

        public string Name { get; private set; }

        public ICollection<CustomerAppliance> Appliances { get; private set; }

        private ApplianceCategory() { }

        public ApplianceCategory(string name, int customerId)
        {
            Name = name;
            CustomerId = customerId;
        }

        public bool HasAppliances() => Appliances?.Any() ?? false;
    }
}
