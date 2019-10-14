using System;

namespace KIOT.Server.Dto.Application.Tasks
{
    public class CustomerTaskDto
    {
        public Guid Guid { get; set; }
        public DateTime StartedAt { get; set; }
        public DateTime? ExpiresAt { get; set; }
        public bool Finished { get; set; }

        public string Title { get; set; }
        public string Description { get; set; }
        public string AssignedBy { get; set; }
    }
}
