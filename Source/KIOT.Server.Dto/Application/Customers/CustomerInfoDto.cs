using System;

namespace KIOT.Server.Dto.Application.Customers
{
    public class CustomerInfoDto
    {
        public Guid CustomerId { get; set; }
        public string Username { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
    }
}
