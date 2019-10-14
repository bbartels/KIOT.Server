using System;

namespace KIOT.Server.Dto.Application.Caretakers.Tasks
{
    public class RegisterCustomerTaskDto
    {
        public Guid CustomerGuid { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime? ExpiresAt { get; set; }
    }
}
