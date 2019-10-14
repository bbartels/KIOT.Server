using System;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;

using KIOT.Server.Business.Request.Application.Caretaker;
using KIOT.Server.Core.Data.Persistence;
using KIOT.Server.Core.Data.Persistence.Application;
using KIOT.Server.Core.Models.Application;
using KIOT.Server.Core.Response;
using KIOT.Server.Core.Services;

namespace KIOT.Server.Business.Handler.Application.Caretakers.Tasks
{
    internal class RegisterCustomerTaskRequestHandler : IRequestHandler<RegisterCustomerTaskRequest, CommandResponse>
    {
        private readonly IUnitOfWork<ICaretakerRepository, Caretaker> _unitOfWork;
        private readonly IValidator<RegisterCustomerTaskRequest> _validator;
        private readonly IPushNotificationService _pushService;

        public RegisterCustomerTaskRequestHandler(IValidator<RegisterCustomerTaskRequest> validator, IPushNotificationService pushService,
            IUnitOfWork<ICaretakerRepository, Caretaker> unitOfWork)
        {
            _validator = validator;
            _pushService = pushService;
            _unitOfWork = unitOfWork;
        }

        public async Task<CommandResponse> Handle(RegisterCustomerTaskRequest request, CancellationToken cancellationToken)
        {
            if (!_validator.Validate(request).IsValid)
            {
                return new CommandResponse(new Error("InvalidRequest", "Invalid Task"));
            }

            var caretaker = await _unitOfWork.Repository.GetByGuidAsync(request.CaretakerGuid,
                $"{nameof(Caretaker.CustomersTasks)}.{nameof(CustomerTask.Customer)}," +
                $"{nameof(Caretaker.TakingCareOf)}.{nameof(CaretakerForCustomer.Customer)}.{nameof(Customer.PushTokens)}");

            if (!caretaker.IsTakingCareOf(request.Dto.CustomerGuid))
            {
                return new CommandResponse(new Error("InvalidRequest", $"No customer with guid: {request.Dto.CustomerGuid} registered."));
            }

            var customer = caretaker.GetCustomer(request.Dto.CustomerGuid);

            caretaker.AddCustomerTask(new CustomerTask(customer, caretaker, request.Dto.Description, request.Dto.Title, request.Dto.ExpiresAt));

            try
            {
                await _unitOfWork.SaveChangesAsync();
                await _pushService.SendNotificationsAsync(customer, "New Target assigned", $"{caretaker.Name} assigned you a new Target!");
            }
            catch (Exception) { return new CommandResponse(new Error("ServerError", "ServerError")); }

            return new CommandResponse();
        }
    }
}
