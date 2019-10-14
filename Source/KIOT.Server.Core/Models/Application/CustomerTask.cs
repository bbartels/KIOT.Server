using System;

namespace KIOT.Server.Core.Models.Application
{
    public class CustomerTask : BaseEntity
    {
        public int CustomerId { get; private set; }
        public Customer Customer { get; private set; }
        public int CaretakerId { get; private set; }
        public Caretaker Caretaker { get; private set; }

        public DateTime StartedAt { get; private set; } = DateTime.UtcNow;
        public DateTime? ExpiresAt { get; private set; }
        public bool Completed { get; private set; }
        public bool TaskFinished => Completed || ExpiresAt < DateTime.UtcNow;

        public string Title { get; private set; }
        public string Description { get; private set; }

        private CustomerTask() { }
        public CustomerTask(Customer customer, Caretaker caretaker, string description, string title, DateTime? expiresAt)
        {
            Customer = customer;
            Caretaker = caretaker;
            Description = description;
            Title = title;
            ExpiresAt = expiresAt;
        }

        public void CompleteTask() => Completed = true;
    }
}
