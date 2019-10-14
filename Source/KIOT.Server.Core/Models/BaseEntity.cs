using System;

namespace KIOT.Server.Core.Models
{
    public class BaseEntity
    {
        public int Id { get; set; }
        public Guid Guid { get; set; } = Guid.NewGuid();
    }
}
