namespace KIOT.Server.Core.Models.Application
{
    public class CustomerAppliance : BaseEntity
    {
        public int ApplianceId { get; private set; }
        public string Alias { get; set; }

        public string Name => Alias ?? ApplianceType.Name;

        public int ApplianceTypeId { get; private set; }
        public ApplianceType ApplianceType { get; private set; }

        public int? CategoryId { get; private set; }
        public ApplianceCategory Category { get; private set; }


        public int CustomerId { get; private set; }
        public Customer Customer { get; private set; }

        private CustomerAppliance() { }

        public CustomerAppliance(int applianceId, string alias, int customerId, ApplianceType applianceType)
        {
            ApplianceId = applianceId;
            Alias = alias;
            CustomerId = customerId;
            ApplianceType = applianceType;
        }

        public void SetCategory(ApplianceCategory category)
        {
            Category = category;
        }
    }
}
