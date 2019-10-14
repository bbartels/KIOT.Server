using System;
using System.Security.Claims;
using FluentValidation;

using KIOT.Server.Core.Response;
using KIOT.Server.Dto.Application.Caretakers.Tasks;

namespace KIOT.Server.Business.Request.Application.Caretaker
{
    public class RegisterCustomerTaskRequest : AuthenticatedCaretakerRequest<CommandResponse>
    {
        public RegisterCustomerTaskDto Dto { get; private set; }

        public RegisterCustomerTaskRequest(RegisterCustomerTaskDto dto, ClaimsIdentity identity) : base(identity)
        {
            Dto = dto;
        }
    }

    public class RegisterCustomerTaskRequestValidator : AbstractValidator<RegisterCustomerTaskRequest>
    {
        public RegisterCustomerTaskRequestValidator()
        {
            RuleFor(x => x.Dto.CustomerGuid).Must(x => x != Guid.Empty);
            RuleFor(x => x.Dto.Description).NotNull().NotEmpty();
            RuleFor(x => x.Dto.Title).NotNull().NotEmpty();
        }
    }
}
