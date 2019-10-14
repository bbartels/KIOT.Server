using System;
using System.Collections.Generic;
using System.Linq;

using KIOT.Server.Dto.Application.Caretakers.Tasks;

namespace KIOT.Server.Core.Models.Application
{
    public class Caretaker : User
    {
        public ICollection<CaretakerForCustomer> TakingCareOf { get; private set; }
        public ICollection<CaretakerForCustomerRequest> TakingCareOfRequests { get; private set; }
        public ICollection<CustomerTask> CustomersTasks { get; private set; }

        public Caretaker(string identityId, string firstName, string lastName, string username, string phoneNumber) 
            : base(identityId, firstName, lastName, username, phoneNumber) { }

        public bool IsTakingCareOf(string customerUsername)
        {
            return TakingCareOf.Any(x => x.Customer.Username == customerUsername);
        }

        public bool IsTakingCareOf(Guid customer)
        {
            return TakingCareOf.Any(x => x.Customer.Guid == customer);
        }

        public bool HasPendingRequest(Guid customer)
        {
            return TakingCareOfRequests.Any(x => x.Customer.Guid == customer);
        }

        public void AddCustomerRequest(int customerId)
        {
            TakingCareOfRequests.Add(new CaretakerForCustomerRequest(Id, customerId));
        }

        public Customer GetCustomer(string username) =>
            TakingCareOf.SingleOrDefault(x => x.Customer.Username == username)?.Customer;

        public Customer GetCustomer(Guid guid) =>
            TakingCareOf.SingleOrDefault(x => x.Customer.Guid == guid)?.Customer;

        public void AddCustomerTask(CustomerTask task)
        {
            if (HasTask(task.Guid)) { return; }
            CustomersTasks.Add(task);
        }

        public IEnumerable<CustomerTask> GetCustomersTasks(Guid guid)
        {
            return CustomersTasks.Where(x => x.Customer.Guid == guid);
        }

        public bool HasTask(Guid taskGuid)
        {
            return CustomersTasks.Any(x => x.Guid == taskGuid);
        }

        public void RemoveTask(CustomerTask task)
        {
            CustomersTasks.Remove(task);
        }

        public CustomerTask GetTask(Guid taskGuid)
        {
            return CustomersTasks.SingleOrDefault(x => x.Guid == taskGuid);
        }

        public void SetTaskState(Guid taskGuid, CustomerTaskState state)
        {
            var task = CustomersTasks.SingleOrDefault(x => x.Guid == taskGuid && !x.TaskFinished);

            if (task == null) { return; }

            if (state == CustomerTaskState.Cancel) { RemoveTask(task); }
            else if (state == CustomerTaskState.Complete) { task.CompleteTask(); }
        }
    }
}
