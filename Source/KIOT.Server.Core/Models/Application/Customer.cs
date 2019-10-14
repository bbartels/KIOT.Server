using System;
using System.Collections.Generic;
using System.Linq;

using KIOT.Server.Core.Models.Data;

namespace KIOT.Server.Core.Models.Application
{
    public class Customer : User
    {
        public string Code { get; private set; }
        public CustomerCode CustomerCode => new CustomerCode(Code);

        public ICollection<CaretakerForCustomer> IsCaredForBy { get; private set; }
        public ICollection<CaretakerForCustomerRequest> IsCaredForByRequests { get; private set; }
        public ICollection<CustomerAppliance> Appliances { get; private set; }
        public ICollection<ApplianceCategory> ApplianceCategories { get; private set; }
        public ICollection<CustomerTask> Tasks { get; private set; }

        private Customer() { }

        public Customer(string identityId, string firstName, string lastName, string username, string customerCode, string phoneNumber) :
            base(identityId, firstName, lastName, username, phoneNumber)
        {
            Code = customerCode;
        }

        public bool IsCaredForByCaretaker(Guid caretakerGuid)
        {
            return IsCaredForBy.Any(c => c.Caretaker.Guid == caretakerGuid);
        }

        public IEnumerable<CustomerTask> GetFinishedTasks() => Tasks.Where(x => x.TaskFinished);
        public IEnumerable<CustomerTask> GetUnfinishedTasks() => Tasks.Where(x => !x.TaskFinished);
        public IEnumerable<CustomerTask> GetTasks() => Tasks;

        public bool HasAppliance(int applianceId)
        {
            return Appliances.Any(x => x.ApplianceId == applianceId);
        }

        public bool HasApplianceCategory(string categoryName)
        {
            return ApplianceCategories.Any(x => x.Name.Equals(categoryName,
                StringComparison.InvariantCultureIgnoreCase));
        }

        public void RemoveApplianceCategory(ApplianceCategory category)
        {
            ApplianceCategories.Remove(category);
        }

        public bool AddApplianceCategory(string categoryName)
        {
            if (ApplianceCategories.Any(x => x.Name.Equals(categoryName,
                StringComparison.InvariantCultureIgnoreCase))) { return false; }
            ApplianceCategories.Add(new ApplianceCategory(categoryName, Id));
            return true;
        }

        public ApplianceCategory GetCategory(Guid categoryGuid)
        {
            return ApplianceCategories.SingleOrDefault(x => x.Guid == categoryGuid);
        }

        public bool ResetApplianceCategory(int applianceId)
        {
            if (!HasAppliance(applianceId)) { return false; }

            var appliance = Appliances.SingleOrDefault(x => x.ApplianceId == applianceId);
            appliance?.SetCategory(null);
            return true;
        }

        public bool SetApplianceCategory(int applianceId, string categoryName)
        {
            if (!HasAppliance(applianceId) || !HasApplianceCategory(categoryName)) { return false; }

            var appliance = Appliances.SingleOrDefault(x => x.ApplianceId == applianceId);
            appliance?.SetCategory(ApplianceCategories.SingleOrDefault(x => x.Name == categoryName));
            return true;
        }

        public bool AddAppliance(CustomerAppliance appliance)
        {
            if (Appliances.Any(x => x.ApplianceId == appliance.ApplianceId)) { return false; }
            Appliances.Add(appliance);
            return true;
        }
    }
}
