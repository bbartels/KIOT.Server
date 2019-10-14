using System;
using System.Security.Claims;
using FluentValidation;

using KIOT.Server.Core.Response;
using KIOT.Server.Dto.Application.Caretakers.Tasks;

namespace KIOT.Server.Business.Request.Application.Caretaker.Tasks
{
    public class SetCustomerTaskStateRequest : AuthenticatedCaretakerRequest<CommandResponse>
    {
        public SetCustomerTaskStateDto Dto { get; private set; }

        public SetCustomerTaskStateRequest(SetCustomerTaskStateDto dto, ClaimsIdentity identity) : base(identity)
        {
            Dto = dto;
        }
    }

    public class SetCustomerTaskStateRequestValidator : AbstractValidator<SetCustomerTaskStateRequest>
    {
        public SetCustomerTaskStateRequestValidator()
        {
            RuleFor(x => x.Dto.CustomerTaskGuid).Must(x => x != Guid.Empty);
            RuleFor(x => x.Dto.State).IsInEnum();
        }
    }
}
