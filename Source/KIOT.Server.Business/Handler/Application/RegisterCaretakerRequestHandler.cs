using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;

using KIOT.Server.Business.Request.Application;
using KIOT.Server.Core.Data.Persistence;
using KIOT.Server.Core.Data.Persistence.Application;
using KIOT.Server.Core.Models.Application;
using KIOT.Server.Core.Response;

namespace KIOT.Server.Business.Handler.Application
{
    internal class RegisterCaretakerRequestHandler : IRequestHandler<RegisterCaretakerRequest, CommandResponse>
    {
        private readonly IValidator<RegisterCaretakerRequest> _validator;
        private readonly IUnitOfWork<IUserRepository, User> _userUnitOfWork;

        public RegisterCaretakerRequestHandler(IUnitOfWork<IUserRepository, User> userUnitOfWork, IValidator<RegisterCaretakerRequest> validator)
        {
            _userUnitOfWork = userUnitOfWork;
            _validator = validator;
        }

        public async Task<CommandResponse> Handle(RegisterCaretakerRequest request, CancellationToken cancellationToken)
        {
            if (!_validator.Validate(request).IsValid)
            {
                return new CommandResponse(new Error("InvalidRequest", "InvalidRequest"));
            }

            var response = await _userUnitOfWork.Repository
                .RegisterCaretaker(request.FirstName, request.LastName, request.Username, request.Email, request.Password, request.PhoneNumber);
            var cmdResponse = new CommandResponse();

            if (response.Errors.Any()) { cmdResponse.AddError("RegisterCaretakerFailed", "Could not register Caretaker."); }
            else { cmdResponse.Message = "Successfully registered Caretaker."; }

            return cmdResponse;
        }
    }
}
