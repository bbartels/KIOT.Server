using System.Collections.Generic;

using KIOT.Server.Core.Response;
using KIOT.Server.Dto.Application.Caretakers;
using KIOT.Server.Dto.Application.Caretakers.Data;

namespace KIOT.Server.Business.Response.Application.Caretakers
{
    public class CustomerHomepageForCaretakerResponse : CommandResponse<CustomerHomepageForCaretakerDto>
    {
        public CustomerHomepageForCaretakerResponse(IEnumerable<Error> errors) : base(errors) { }

        public CustomerHomepageForCaretakerResponse(CustomerHomepageForCaretakerDto model) : base(model) { }
    }
}
