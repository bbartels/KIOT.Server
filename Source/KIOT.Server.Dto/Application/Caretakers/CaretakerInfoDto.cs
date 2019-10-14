using System;

namespace KIOT.Server.Dto.Application.Caretakers
{
    public class CaretakerInfoDto
    {
        public Guid CustomerId { get; set; }
        public string Username { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
    }
}
