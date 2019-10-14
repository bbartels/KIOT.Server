using System;

namespace KIOT.Server.Dto.Application.Caretakers.Tasks
{
    public enum CustomerTaskState : byte
    {
        Complete,
        Cancel
    }

    public class SetCustomerTaskStateDto
    {
        public Guid CustomerTaskGuid { get; set; }
        public CustomerTaskState State { get; set; }
    }
}
