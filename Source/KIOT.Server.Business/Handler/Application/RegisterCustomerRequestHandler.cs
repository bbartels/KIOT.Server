using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;

using KIOT.Server.Business.Request.Application;
using KIOT.Server.Core.Data.Api.Request;
using KIOT.Server.Core.Data.Persistence;
using KIOT.Server.Core.Data.Persistence.Application;
using KIOT.Server.Core.Models.Application;
using KIOT.Server.Core.Response;

namespace KIOT.Server.Business.Handler.Application
{
    internal class RegisterCustomerRequestHandler : IRequestHandler<RegisterCustomerRequest, CommandResponse>
    {
        private readonly IValidator<RegisterCustomerRequest> _validator;
        private readonly IUnitOfWork<IUserRepository, User> _userUnitOfWork;
        private readonly IApiClient _apiClient;

        public RegisterCustomerRequestHandler(IUnitOfWork<IUserRepository, User> userUnitOfWork, IApiClient client, IValidator<RegisterCustomerRequest> validator)
        {
            _userUnitOfWork = userUnitOfWork;
            _apiClient = client;
            _validator = validator;
        }

        public async Task<CommandResponse> Handle(RegisterCustomerRequest request, CancellationToken cancellationToken)
        {
            if (!_validator.Validate(request).IsValid)
            {
                return new CommandResponse(new Error("InvalidRequest", "InvalidRequest"));
            }

            if (!await _apiClient.CustomerExists(request.CustomerCode))
            {
                return new CommandResponse(new Error("RegisterCustomerFailed", "CustomerId does not exist."));
            }

            var response = await _userUnitOfWork.Repository
                .RegisterCustomer(request.FirstName, request.LastName, request.Username, request.Email,
                    request.Password, request.CustomerCode.Code, request.PhoneNumber);

            var cmdResponse = response.Errors.Any()
                ? new CommandResponse(response.Errors)
                : new CommandResponse { Message = "Successfully registered Customer." };

            return cmdResponse;
        }
    }
}
