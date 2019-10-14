using System.Collections.Generic;

using KIOT.Server.Core.Response;
using KIOT.Server.Dto.Application.Customers;
using KIOT.Server.Dto.Application.Customers.Caretakers;

namespace KIOT.Server.Business.Response.Application.Customers
{
    public class GetCaretakersForCustomerResponse : CommandResponse<AssignedCaretakersForCustomerDto>
    {
        public GetCaretakersForCustomerResponse(IEnumerable<Error> errors) : base(errors) { }

        public GetCaretakersForCustomerResponse(AssignedCaretakersForCustomerDto model) : base(model) { }
    }
}
